ALTER TABLE airports
ADD CONSTRAINT unique_old_airport_id UNIQUE (old_airport_id);

ALTER TABLE runways
ADD CONSTRAINT fk_runways_airports
FOREIGN KEY (fk_airport_id) REFERENCES airports(old_airport_id);

CREATE INDEX idx_fk_airport_id ON runways(fk_airport_id);