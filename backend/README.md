﻿# PokéGame

Experimenting a Pokémon Tabletop Role-Playing Game.

## Environment Variables

All the following environment variables are optional.

- `ADMIN_BASE_PATH`: the base path of the administration interface. Defaults to `admin`.
- `ADMIN_ENABLE_SWAGGER`: a boolean value indicating whether or not to enable Swagger UI. Should not be enabled in Production environment for security purposes. Defaults to `false`.
- `ADMIN_TITLE`: the title of the API, used for `/api` route and Swagger UI. Defaults to `PokéGame API`.
- `ADMIN_VERSION`: the version of the API, used for `/api` route and Swagger UI. Defaults to current API version.
- `AUTHENTICATION_ACCESS_TOKEN_LIFETIME_SECONDS`: the default lifetime of access tokens, in seconds. Defaults to `300` (5 minutes).
- `AUTHENTICATION_ACCESS_TOKEN_TYPE`: the token type of access tokens. Defaults to `at+jwt`.
- `AUTHENTICATION_ENABLE_BASIC`: a boolean value indicating whether or not to enable Basic authentication. API keys should be preferred in Production environment for security purposes. Defaults to `false`.
- `CACHING_ACTOR_LIFETIME`: the lifetime of cached actors. A string representing a [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan?view=net-9.0), ex.: `3.00:00:00` (3 days) or `00:15:00` (15 minutes).
- `DATABASE_APPLY_MIGRATIONS`: a boolean value indicating whether or not to apply database migrations on application startup. Manually applying SQL Scripts should be preferred in Production environment for data safety purposes. Defaults to `false`.
- `DATABASE_ENABLE_LOGGING`: a boolean value indicating whether or not to store logs into the database. This should be disabled in Production environment to avoid bloating the database. You should use another provider, such as MongoDB. Defaults to `false`.
- `DATABASE_PROVIDER`: the database provider to use. Its value should be one of the `DatabaseProvider` enumeration value. Defaults to `EntityFrameworkCorePostgreSQL`.
- `DEFAULT_LOCALE`: the default locale code of the system. A string representing a [CultureInfo](https://learn.microsoft.com/en-us/dotnet/api/system.globalization.cultureinfo?view=net-9.0), ex.: `en` (English) or `fr-CA` (Canadian French).
- `DEFAULT_PASSWORD`: the default password of the admin user, ex.: `P@s$W0rD`.
- `DEFAULT_USERNAME`: the default username of the admin user, ex.: `admin`.
- `ENCRYPTION_KEY`: the encryption key used by the platform. It should be 32-characters long (256 bits), including lowercase and uppercase letters, digits and special characters as well.
- `ERROR_EXPOSE_DETAIL`: a boolean value indicating whether or not to expose detail for `500 Internal Server Error`. Should not be enabled in Production environment for security purposes.
- `MONGOCONNSTR_Pokemon`: the MongoDB server connection string. This is currently only used for logging.
- `MONGODB_DATABASE_NAME`: the MongoDB database name. This is currently only used for logging. Defaults to `pokegame`.
- `MONGODB_ENABLE_LOGGING`: a boolean value indicating whether or not to store logs into MongoDB. This should be preferred to database logging in Production environment to avoid bloating the database. Defaults to `false`.
- `PASSWORDS_PBKDF2_ALGORITHM`: the hashing algorithm for PBKDF2 passwords. Defaults to `HMACSHA256`.
- `PASSWORDS_PBKDF2_HASH_LENGTH`: the hash length (in bytes) for PBKDF2. When not specified, will default to salt length.
- `PASSWORDS_PBKDF2_ITERATIONS`: the hashing iterations for PBKDF2 passwords. Defaults to 600000.
- `PASSWORDS_PBKDF2_SALT_LENGTH`: the salt length (in bytes) for PBKDF2 passwords. Defaults to 32 (256 bits).
- `POSTGRESQLCONNSTR_Pokemon`: the PostgreSQL connection string.
