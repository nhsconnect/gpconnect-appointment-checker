CREATE SCHEMA audit;

CREATE SCHEMA configuration;

CREATE SCHEMA logging;

CREATE TABLE logging.exception
( 
    Id serial primary key,
    Application character varying(100) NULL,
    Logged text,
    Level character varying(100) NULL,
    Message character varying(8000) NULL,
    Logger character varying(8000) NULL, 
    Callsite character varying(8000) NULL, 
    Exception character varying(8000) NULL
)

CREATE TABLE audit.entry (
    entry_id integer NOT NULL,
    ip character varying(1000) NOT NULL,
    user_name character varying(255) NOT NULL,
    ods_code character varying(255),
    created_date date NOT NULL,
    logged_on date,
    logged_off date,
    description character varying(1000) NOT NULL
);

CREATE TABLE audit.entry_type (
    entry_type_id integer NOT NULL,
    description character varying(1000) NOT NULL
);


CREATE TABLE configuration.entry (
    entry_id integer NOT NULL,
    key character varying(255) NOT NULL,
    value character varying(1000) NOT NULL
);

CREATE TABLE logging.entry (
    entry_id integer NOT NULL,
    url character varying(1000) NOT NULL,
    referrer_url character varying(1000),
    description character varying(1000) NOT NULL,
    ip character varying(255) NOT NULL,
    created_date date NOT NULL,
    created_by character varying(255) NOT NULL,
    server character varying(255) NOT NULL,
    response_code integer NOT NULL,
    session_id character varying(1000) NOT NULL,
    user_agent character varying(1000) NOT NULL
);

CREATE TABLE logging.entry_type (
    entry_type_id integer NOT NULL,
    description character varying(1000) NOT NULL
);

CREATE SEQUENCE audit.entry_entry_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;

CREATE SEQUENCE configuration.entry_entry_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;

CREATE SEQUENCE logging.entry_entry_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;

ALTER TABLE ONLY audit.entry ALTER COLUMN entry_id SET DEFAULT nextval('audit.entry_entry_id_seq'::regclass);
ALTER TABLE ONLY configuration.entry ALTER COLUMN entry_id SET DEFAULT nextval('configuration.entry_entry_id_seq'::regclass);
ALTER TABLE ONLY logging.entry ALTER COLUMN entry_id SET DEFAULT nextval('logging.entry_entry_id_seq'::regclass);
ALTER TABLE ONLY audit.entry ADD CONSTRAINT audit_entry_entryid_pk PRIMARY KEY (entry_id);
ALTER TABLE ONLY audit.entry_type ADD CONSTRAINT audit_entrytype_entrytypeid_pk PRIMARY KEY (entry_type_id);
ALTER TABLE ONLY configuration.entry ADD CONSTRAINT configuration_entry_entryid_pk PRIMARY KEY (entry_id);
ALTER TABLE ONLY configuration.entry ADD CONSTRAINT unique_configuration_key UNIQUE (key);
ALTER TABLE ONLY logging.entry ADD CONSTRAINT logging_entry_entryid_pk PRIMARY KEY (entry_id);
ALTER TABLE ONLY logging.entry_type ADD CONSTRAINT logging_entrytype_entrytypeid_pk PRIMARY KEY (entry_type_id);
ALTER TABLE ONLY audit.entry ADD CONSTRAINT audit_entry_entryid_fk FOREIGN KEY (entry_id) REFERENCES audit.entry_type(entry_type_id);
ALTER TABLE ONLY logging.entry ADD CONSTRAINT logging_entry_entryid_fk FOREIGN KEY (entry_id) REFERENCES logging.entry_type(entry_type_id);