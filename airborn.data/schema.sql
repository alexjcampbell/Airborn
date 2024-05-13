--
-- PostgreSQL database dump
--

-- Dumped from database version 15.2
-- Dumped by pg_dump version 15.2

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: airports; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.airports (
    airport_id integer NOT NULL,
    old_airport_id integer,
    ident text,
    type text,
    name text,
    latitude_deg numeric,
    longitude_deg numeric,
    elevation_ft integer,
    continent text,
    iso_country text,
    iso_region text,
    municipality text,
    scheduled_service text,
    gps_code text,
    iata_code text,
    local_code text,
    home_link text,
    wikipedia_link text,
    keywords text,
    magnetic_variation integer
);


ALTER TABLE public.airports OWNER TO postgres;

--
-- Name: airports_new_airport_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.airports_new_airport_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.airports_new_airport_id_seq OWNER TO postgres;

--
-- Name: airports_new_airport_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.airports_new_airport_id_seq OWNED BY public.airports.new_airport_id;


--
-- Name: runways; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.runways (
    runway_id integer NOT NULL,
    old_runway_id integer,
    fk_airport_id integer,
    airport_ident text,
    length_ft integer,
    width_ft integer,
    surface text,
    lighted text,
    closed text,
    runway_name text,
    latitude_deg double precision,
    longitude_deg double precision,
    elevation_ft integer,
    heading_degt numeric,
    displaced_threshold_ft integer,
    surface_friendly text,
    old_id bigint,
    fk_new_airport_id integer
);


ALTER TABLE public.runways OWNER TO postgres;

--
-- Name: runways_new_runway_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.runways_new_runway_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.runways_new_runway_id_seq OWNER TO postgres;

--
-- Name: runways_new_runway_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.runways_new_runway_id_seq OWNED BY public.runways.new_runway_id;


--
-- Name: airports new_airport_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.airports ALTER COLUMN new_airport_id SET DEFAULT nextval('public.airports_new_airport_id_seq'::regclass);


--
-- Name: runways new_runway_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.runways ALTER COLUMN new_runway_id SET DEFAULT nextval('public.runways_new_runway_id_seq'::regclass);


--
-- Name: airports airports_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.airports
    ADD CONSTRAINT airports_pkey PRIMARY KEY (new_airport_id);


--
-- Name: runways runways_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.runways
    ADD CONSTRAINT runways_pkey PRIMARY KEY (new_runway_id);


--
-- Name: airports unique_old_airport_id; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.airports
    ADD CONSTRAINT unique_old_airport_id UNIQUE (old_airport_id);


--
-- Name: idx_fk_airport_id; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_fk_airport_id ON public.runways USING btree (fk_airport_id);


--
-- Name: runways fk_runways_airports; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.runways
    ADD CONSTRAINT fk_runways_airports FOREIGN KEY (fk_airport_id) REFERENCES public.airports(old_airport_id);


--
-- Name: TABLE airports; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.airports TO airborn_user;


--
-- Name: SEQUENCE airports_new_airport_id_seq; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON SEQUENCE public.airports_new_airport_id_seq TO airborn_user;


--
-- Name: TABLE runways; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.runways TO airborn_user;


--
-- Name: SEQUENCE runways_new_runway_id_seq; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON SEQUENCE public.runways_new_runway_id_seq TO airborn_user;


--
-- Name: DEFAULT PRIVILEGES FOR SEQUENCES; Type: DEFAULT ACL; Schema: public; Owner: postgres
--

ALTER DEFAULT PRIVILEGES FOR ROLE postgres IN SCHEMA public GRANT ALL ON SEQUENCES  TO airborn_user;


--
-- Name: DEFAULT PRIVILEGES FOR TABLES; Type: DEFAULT ACL; Schema: public; Owner: postgres
--

ALTER DEFAULT PRIVILEGES FOR ROLE postgres IN SCHEMA public GRANT ALL ON TABLES  TO airborn_user;


--
-- PostgreSQL database dump complete
--

