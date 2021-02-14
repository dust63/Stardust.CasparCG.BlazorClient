##To initialize db

go to Server/ScheduleEngine


to unappply all
dotnet ef database update 0

to create migration
dotnet ef migrations add InitialCreate

to pass update
dotnet ef database update

##RESET DB POSTGRE
DROP SCHEMA public CASCADE;
CREATE SCHEMA public;
GRANT ALL ON SCHEMA public TO public;


to use Publish Api you need to set

dotnet user-secrets set "YoutubeApi:ClientId" "<your client id>"
<your client secrets>"

test for send youtube data
{
  "channelId": "3be4c75f-acd4-491b-bf69-941125e24ea5",
  "title": "Ma super video",
  "description": "Ma super video description",
  "tags": [
    "hello"
  ],
  "categoryId": "22",
  "privacyStatus": "private",
  "filePath": "D:\\CasparDatas\\media\\AMB.mp4"
}