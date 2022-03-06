db = db.getSiblingDB('api_prod_db');
db.createUser(
    {
        user: 'api_user',
        pwd: 'api1234',
        roles: [{ role: 'readWrite', db: 'api_prod_db' }],
    },
);