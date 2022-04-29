CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    migration_id character varying(150) NOT NULL,
    product_version character varying(32) NOT NULL,
    CONSTRAINT pk___ef_migrations_history PRIMARY KEY (migration_id)
);

START TRANSACTION;

CREATE TABLE accounts (
    id uuid NOT NULL,
    name text NOT NULL,
    email text NOT NULL,
    password_hash text NOT NULL,
    date_created timestamp with time zone NOT NULL,
    access_failed_count integer NOT NULL,
    lockout_end timestamp with time zone NULL,
    verification_code text NULL,
    verification_code_expires timestamp with time zone NULL,
    email_confirmed boolean NOT NULL,
    phone_number text NULL,
    phone_number_confirmed boolean NOT NULL,
    CONSTRAINT pk_accounts PRIMARY KEY (id)
);

CREATE TABLE refresh_tokens (
    id uuid NOT NULL,
    token text NOT NULL,
    replaced_by_token text NULL,
    date_created timestamp with time zone NOT NULL,
    date_expires timestamp with time zone NOT NULL,
    date_revoked timestamp with time zone NULL,
    account_id uuid NOT NULL,
    CONSTRAINT pk_refresh_tokens PRIMARY KEY (id),
    CONSTRAINT fk_refresh_tokens_accounts_account_id FOREIGN KEY (account_id) REFERENCES accounts (id) ON DELETE CASCADE
);

CREATE UNIQUE INDEX ix_accounts_email ON accounts (email);

CREATE INDEX ix_refresh_tokens_account_id ON refresh_tokens (account_id);

CREATE UNIQUE INDEX ix_refresh_tokens_token ON refresh_tokens (token);

INSERT INTO "__EFMigrationsHistory" (migration_id, product_version)
VALUES ('20220429194518_InitialMigration', '6.0.4');

COMMIT;

