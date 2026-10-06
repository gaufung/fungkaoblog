\set ON_ERROR_STOP on
\getenv blog_password BLOG_DATABASE_PASSWORD

SELECT format(
    'CREATE ROLE fungkaoblog_app LOGIN NOSUPERUSER NOCREATEDB NOCREATEROLE NOREPLICATION PASSWORD %L',
    :'blog_password'
)
WHERE NOT EXISTS (SELECT 1 FROM pg_roles WHERE rolname = 'fungkaoblog_app')
\gexec

ALTER DATABASE fungkaoblog OWNER TO fungkaoblog_app;
REVOKE ALL ON DATABASE fungkaoblog FROM PUBLIC;

\connect fungkaoblog
REVOKE CREATE ON SCHEMA public FROM PUBLIC;
GRANT USAGE, CREATE ON SCHEMA public TO fungkaoblog_app;
