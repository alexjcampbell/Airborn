# Airborn
Check out <a href="https://www.airborn.co">https;//airborn.co</a> to run this in production!

Instructions for updating production data:
pg_dump --clean --if-exists -h localhost -U my_local_user mydb > mydb.sql
psql $(heroku config:get DATABASE_URL -a my-heroku-app) < mydb.sql
