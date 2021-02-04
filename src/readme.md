##To initialize db

go to Server/ScheduleEngine

to create migration
dotnet ef migrations add InitialCreate

to pass update
dotnet ef database update

##RESET DB POSTGRE
DROP SCHEMA public CASCADE;
CREATE SCHEMA public;
GRANT ALL ON SCHEMA public TO public;


to use Publish Api you need to set
dotnet user-secrets set "YoutubeApi:ClientSecrets" "<your client secrets>
dotnet user-secrets set "YoutubeApi:ClientId" "<your client id>