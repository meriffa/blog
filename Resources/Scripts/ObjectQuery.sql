-- Create Tables
CREATE TABLE "ObjectQueryTemp" ("Data" jsonb);
CREATE TABLE "ObjectQuery" ("Address" bigint, AddressHex varchar(18), Type text);

-- Load Data
INSERT INTO "ObjectQuery" SELECT ("Data"->>'Address')::bigint, "Data"->>'AddressHex', "Data"->>'Type' FROM "ObjectQueryTemp";

-- Cleanup
TRUNCATE TABLE "ObjectQueryTemp";
DROP TABLE "ObjectQueryTemp";