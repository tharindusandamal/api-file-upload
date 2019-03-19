# api-file-upload
file upload using API controller

Here I created a .NET core application, to upload files using api controller, here we can upload multipart files, and JSONs, seriver will save all the documents in tempory document directory that i created. we can send the files from postman, fiddler or any api-client application.

Url: /api/documents
Type: POST
Content-type: multipart/form-data

Request should be as follows, (fiddler)

HEADER:
Content-Type: multipart/form-data; boundary=-------------------------acebdf13572468

BODY:
---------------------------acebdf13572468
Content-Disposition: form-data; name="fieldNameHere"; filename="1.pdf"
Content-Type: application/pdf

<@INCLUDE *C:\Users\tharindu.ENADOC\Documents\Sample-PDF\1.pdf*@>
---------------------------acebdf13572468--
Content-Type: application/json

{"sample":"data"}
---------------------------acebdf13572468--
