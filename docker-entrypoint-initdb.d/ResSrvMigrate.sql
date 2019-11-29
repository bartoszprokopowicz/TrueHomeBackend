CREATE DATABASE "TrueHomeDB";

\c "TrueHomeDB"

CREATE TABLE "role" (
	ID_Role       SERIAL,
	RoleName      varchar (30) NOT NULL,
	CONSTRAINT pk_role PRIMARY KEY (ID_Role)
);

CREATE TABLE "user" (
	ID_User       text NOT NULL,
	Login         varchar (100) NOT NULL,
	Email         varchar (100) NOT NULL,
	Rate 		  numeric (2,1),
	isBlocked	  boolean NOT NULL,
	IDRole        INTEGER,
	CONSTRAINT pk_user PRIMARY KEY(ID_User),
	CONSTRAINT fk_role FOREIGN KEY(IDRole) REFERENCES "role"(ID_Role) ON DELETE RESTRICT
);

CREATE TABLE "apartment" (
	ID_Ap         		SERIAL,
	Name           		varchar (100) NOT NULL,
	City            	varchar (100) NOT NULL,
	Street          	varchar (100) NOT NULL,
	ApartmentNumber 	varchar (20)  NOT NULL,
	ImgThumb        	varchar (200),
	ImgList         	varchar (200)[],
	OwnerRatingSum  	INTEGER DEFAULT 0,
	LocationRatingSum  	INTEGER DEFAULT 0,
	StandardRatingSum  	INTEGER DEFAULT 0,
	PriceRatingSum     	INTEGER DEFAULT 0,
	RatingsCount		INTEGER DEFAULT 0,
	Lat             	numeric (9,7) NOT NULL,
	Long            	numeric (10,7)NOT NULL,
	Description 		text,
	IDUser          	text NOT NULL,
	CONSTRAINT pk_apartment PRIMARY KEY(ID_Ap),
	CONSTRAINT fk_user FOREIGN KEY(IDUser) REFERENCES "user"(ID_User) ON DELETE RESTRICT
);

CREATE TABLE "renting" (
	ID_Renting		SERIAL,
	IDUser			text NOT NULL,
	IDAp			INTEGER NOT NULL,
	date_from		Date NOT NULL,
	date_to 		Date,
	CONSTRAINT pk_renting PRIMARY KEY (ID_Renting),
	CONSTRAINT fk_user FOREIGN KEY(IDUser) REFERENCES "user"(ID_User) ON DELETE RESTRICT,
	CONSTRAINT fk_apartment FOREIGN KEY(IDAp) REFERENCES "apartment"(ID_Ap) ON DELETE RESTRICT
);

CREATE TABLE "rating" (
	ID_Rating		SERIAL,
	Owner			numeric (1) NOT NULL,
	Location		numeric (1) NOT NULL,
	Standard		numeric (1) NOT NULL,
	Price			numeric (1) NOT NULL,
	Description		text,
	IDRenting		INTEGER NOT NULL,
	IDAp			INTEGER NOT NULL,
	CONSTRAINT pk_rating PRIMARY KEY (ID_Rating),
	CONSTRAINT fk_renting FOREIGN KEY (IDRenting) REFERENCES "renting"(ID_Renting) ON DELETE RESTRICT,
	CONSTRAINT fk_ap FOREIGN KEY (IDAp) REFERENCES "apartment"(ID_Ap) ON DELETE RESTRICT
);
	
CREATE TABLE "personaldata" (
	ID_PData	SERIAL,
	FirstName	varchar(50) NOT NULL,
	LastName	varchar(100) NOT NULL,
	BirthDate	Date,
	PhoneNumber varchar(20),
	IDUser		text NOT NULL,
	CONSTRAINT pk_personaldata PRIMARY KEY(ID_PData),
	CONSTRAINT fk_user FOREIGN KEY(IDUser) REFERENCES "user"(ID_User) ON DELETE RESTRICT
);

CREATE TABLE "dump" (
	ID_PData	SERIAL
);

CREATE TYPE apartment_avg AS (
	ID_Ap         		INTEGER,
	Name           		varchar (100),
	City            	varchar (100),
	Street          	varchar (100),
	ApartmentNumber 	varchar (20),
	ImgThumb        	varchar (200),
	ImgList         	varchar (200)[],
	OwnerRating  		double precision,
	LocationRating  	double precision,
	StandardRating  	double precision,
	PriceRating     	double precision,
	PhoneNumber			varchar(20),
	Lat             	numeric (9,7),
	Long            	numeric (10,7),
	Description 		text,
	IDUser          	text
);

CREATE OR REPLACE FUNCTION get_apartment(id integer) 
 RETURNS apartment_avg
AS $$
DECLARE
	result_record apartment_avg;
BEGIN
 SELECT 
	ID_Ap,
	Name,
	City,
	Street,
	ApartmentNumber,
	ImgThumb,
	ImgList,
	OwnerRatingSum/NULLIF(RatingsCount,0)::double precision AS OwnerRating,
	LocationRatingSum/NULLIF(RatingsCount,0)::double precision AS LocationRating,
	StandardRatingSum/NULLIF(RatingsCount,0)::double precision AS StandardRating,
	PriceRatingSum/NULLIF(RatingsCount,0)::double precision AS PriceRating,
	(SELECT PhoneNumber FROM PersonalData pd WHERE pd.IDUser = ap.IDUser),
	Lat,
	Long,
	Description,
	IDUser
 INTO result_record
 FROM apartment ap
 WHERE ID_Ap=id;
 
 RETURN result_record;
END; $$ 
LANGUAGE 'plpgsql';

CREATE OR REPLACE FUNCTION get_all_apartments()
 RETURNS SETOF apartment_avg
AS $$
BEGIN
 RETURN QUERY SELECT 
	ID_Ap,
	Name,
	City,
	Street,
	ApartmentNumber,
	ImgThumb,
	ImgList,
	OwnerRatingSum/NULLIF(RatingsCount,0)::double precision AS OwnerRating,
	LocationRatingSum/NULLIF(RatingsCount,0)::double precision AS LocationRating,
	StandardRatingSum/NULLIF(RatingsCount,0)::double precision AS StandardRating,
	PriceRatingSum/NULLIF(RatingsCount,0)::double precision AS PriceRating,
	(SELECT PhoneNumber FROM PersonalData pd WHERE pd.IDUser = ap.IDUser),
	Lat,
	Long,
	Description,
	IDUser
 FROM apartment ap;
END; $$ 
LANGUAGE 'plpgsql';


CREATE OR REPLACE FUNCTION update_ratings()
	RETURNS TRIGGER AS $add_ratings$
DECLARE
   OwnerDiff	INTEGER := 0;
   LocationDiff	INTEGER := 0;
   StandardDiff	INTEGER := 0;
   PriceDiff	INTEGER := 0;
   CountDiff	INTEGER := 0;
BEGIN
	CASE TG_OP
		WHEN 'INSERT' THEN
			OwnerDiff = NEW.Owner;
			LocationDiff = NEW.Location;
			StandardDiff = NEW.Standard;
			PriceDiff = NEW.Price;
			CountDiff = 1;
		WHEN 'UPDATE' THEN
			OwnerDiff = NEW.Owner - OLD.Owner;
			LocationDiff = NEW.Location - OLD.Location;
			StandardDiff = NEW.Standard - OLD.Standard;
			PriceDiff = NEW.Price - OLD.Price;
		WHEN 'DELETE' THEN
			OwnerDiff = - OLD.Owner;
			LocationDiff = - OLD.Location;
			StandardDiff = - OLD.Standard;
			PriceDiff = - OLD.Price;
			CountDiff = -1;
	END CASE;
	
   UPDATE apartment
		SET
			OwnerRatingSum = OwnerRatingSum + OwnerDiff,
			LocationRatingSum = LocationRatingSum + LocationDiff,
			StandardRatingSum = StandardRatingSum + StandardDiff,
			PriceRatingSum = PriceRatingSum + PriceDiff,
			RatingsCount = RatingsCount + CountDiff
		WHERE
			ID_Ap = NEW.IDAp;
   RETURN NEW;
END
$add_ratings$ LANGUAGE plpgsql;
			
CREATE TRIGGER rating_update
    AFTER INSERT OR UPDATE OR DELETE ON rating
    FOR EACH ROW EXECUTE PROCEDURE update_ratings();
