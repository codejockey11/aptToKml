DEL aptToKml.txt

DEL aptAirport.kml
DEL aptHeliport.kml
DEL aptRunway.kml
DEL aptWaterport.kml

mysql.exe --login-path=batch --table < aptToKml.sql

aptToKml.exe aptToKml.txt

REM DEL aptToKml.txt

REM SET GDAL_DATA=C:\\Program Files\\QGIS 3.22.1\\apps\\gdal-dev\\data
REM SET GDAL_DRIVER_PATH=C:\\Program Files\\QGIS 3.22.1\\bin\\gdalplugins
REM SET OSGEO4W_ROOT=C:\\Program Files\\QGIS 3.22.1
REM SET PATH=%OSGEO4W_ROOT%\\bin;%PATH%
REM SET PYTHONHOME=%OSGEO4W_ROOT%\\apps\\Python37
REM SET PYTHONPATH=%OSGEO4W_ROOT%\\apps\\Python37

REM ogr2ogr.exe -f "ESRI Shapefile" -skipfailures "shape\\aptAirport.shp" "aptAirport.kml" aptAirport
REM ogr2ogr.exe -f "ESRI Shapefile" -skipfailures "shape\\aptHeliport.shp" "aptHeliport.kml" aptHeliport
REM ogr2ogr.exe -f "ESRI Shapefile" -skipfailures "shape\\aptRunway.shp" "aptRunway.kml" aptRunway
REM ogr2ogr.exe -f "ESRI Shapefile" -skipfailures "shape\\aptWaterport.shp" "aptWaterport.kml" aptWaterport
