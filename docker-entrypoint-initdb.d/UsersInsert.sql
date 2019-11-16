INSERT INTO "AspNetUsers"
VALUES ('fddb44a3-43ae-44e2-b8a2-0962fa6be039', 'a@a.a', 'A@A.A', 'a@a.a', 'A@A.A', 'f', 'AQAAAAEAACcQAAAAEJwHuI9fREiOjP1gpbhmIH4UMJD+YIlvcR9Od5uvIPTWXp8KjXLCk6ng1PVqBPV91A==', 'CSOETKFG3JI46PNL32HOBWZDGEFAWCPN', 'cc4b6777-0e78-490a-a248-e503babbafc0', NULL, 'f', 'f', NULL, 't', 0 );

\c "TrueHomeDB"

INSERT INTO "role" (RoleName)
VALUES ('admin');

INSERT INTO "user"(ID_User, Login, Email, isBlocked, IDRole)
VALUES ('fddb44a3-43ae-44e2-b8a2-0962fa6be039','a@a.a','a@a.a','f',1);

INSERT INTO "personaldata"(FirstName, LastName, BirthDate, IDUser)
VALUES('Jan', 'Kowalski', '1990-06-01','fddb44a3-43ae-44e2-b8a2-0962fa6be039');

INSERT INTO "apartment" (name,city,street,apartmentNumber,imgthumb,imglist,lat,long,iduser)
VALUES 
('luksus','Wrocław','Sienkiewicza','21/31','https://media-cdn.trulia-local.com/neighborhood-media-service-prod/il/chicago/south-loop/920-il_chi_south_loop_269607_177_256x256_cfill.jpg','{1.jpg,2.jpg}','-39.98894','27.16119','fddb44a3-43ae-44e2-b8a2-0962fa6be039'),
('mieszkanko','D�gelis','Ap #988-1740 Nunc Rd.','653-7584 Tortor, St.','https://media-cdn.trulia-local.com/neighborhood-media-service-prod/tx/austin/south-lamar/282-tx_aus_south_lamar_271570_2350_256x256_cfill.jpg','{1.jpg,2.jpg}','19.10539','11.74472','fddb44a3-43ae-44e2-b8a2-0962fa6be039'),
('domek','Le Puy-en-Velay','6353 Ut, Ave','5','http://www.gatewayapartments.com.hk/img/GatewayApartments.png','{1.jpg,2.jpg}','25.23089','112.71901','fddb44a3-43ae-44e2-b8a2-0962fa6be039'),
('zameczek na nadodrzu','Purranque','Ap #891-8040 Nec Rd.','2','https://media-cdn.trulia-local.com/neighborhood-media-service-prod/il/chicago/south-loop/920-il_chi_south_loop_269607_177_256x256_cfill.jpg','{1.jpg,2.jpg}','-3.07401','-93.0161','fddb44a3-43ae-44e2-b8a2-0962fa6be039'),
('kawalerka w piwnicy','Dutse','813-1033 Vitae Rd.','10A ','https://media-cdn.trulia-local.com/neighborhood-media-service-prod/tx/austin/south-lamar/282-tx_aus_south_lamar_271570_2350_256x256_cfill.jpg','{1.jpg,2.jpg}','18.29239','-132.24146','fddb44a3-43ae-44e2-b8a2-0962fa6be039');

INSERT INTO "rating" (Owner,Location,Standard,Price,Description,IDUser,IDAp)
VALUES
(4,5,1,2,'takie se','fddb44a3-43ae-44e2-b8a2-0962fa6be039',1),
(5,1,5,4,'takie se','fddb44a3-43ae-44e2-b8a2-0962fa6be039',2),
(3,3,3,3,'takie se','fddb44a3-43ae-44e2-b8a2-0962fa6be039',3),
(2,5,2,5,'takie se','fddb44a3-43ae-44e2-b8a2-0962fa6be039',4),
(1,4,3,1,'takie se','fddb44a3-43ae-44e2-b8a2-0962fa6be039',5),
(4,5,4,2,'takie se','fddb44a3-43ae-44e2-b8a2-0962fa6be039',1),
(3,1,3,1,'takie se','fddb44a3-43ae-44e2-b8a2-0962fa6be039',2),
(3,2,3,1,'takie se','fddb44a3-43ae-44e2-b8a2-0962fa6be039',3),
(4,5,1,5,'takie se','fddb44a3-43ae-44e2-b8a2-0962fa6be039',4),
(5,5,1,4,'takie se','fddb44a3-43ae-44e2-b8a2-0962fa6be039',5);