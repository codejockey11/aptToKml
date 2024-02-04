USE aviation;

SELECT apt.ICAO,apt.name,apt.latitude AS aptlatitude,apt.longitude AS aptlongitude,apt.magvar,rwy.runwayid,rwy.surface,rwy.length,rwy.latitude,rwy.longitude
	INTO OUTFILE 'C:\\Users\\junk_\\Documents\\qgisBatch\\aptToKml.txt' FIELDS TERMINATED BY '~' LINES TERMINATED BY '\r\n'
	FROM aptairport apt, aptrunway rwy
	WHERE apt.facilityid=rwy.facilityid;
