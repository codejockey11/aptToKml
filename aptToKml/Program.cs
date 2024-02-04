using System;
using System.IO;
using aviationLib;

namespace aptToKml
{
    class Program
    {
        static StreamReader rowset;

        static String[] columns;
        static String row;

        static readonly StreamWriter aptAirport = new StreamWriter("aptAirport.kml", false);
        static readonly StreamWriter aptHeliport = new StreamWriter("aptHeliport.kml", false);
        static readonly StreamWriter aptRunway = new StreamWriter("aptRunway.kml", false);
        static readonly StreamWriter aptWaterport = new StreamWriter("aptWaterport.kml", false);

        static bool wrotePoint = false;

        static String prevICAO;

        static LatLon latLon;

        static void WriteRunway()
        {
            if (columns[8] == "")
            {
                latLon = new LatLon(columns[2], columns[3]);
            }
            else
            {
                latLon = new LatLon(columns[8], columns[9]);
            }

            String[] runways = columns[5].Split('/');

            if (runways[0].Length == 1)
            {
                if ((runways[0] == "N") || (runways[0] == "S"))
                {
                    runways[0] = "1";
                }

                if ((runways[0] == "E") || (runways[0] == "W"))
                {
                    runways[0] = "9";
                }
            }
            else
            {
                String heading = columns[5].Replace("ALL/WAY", "18/36");

                heading = heading.Replace("N", "");
                heading = heading.Replace("S", "");
                heading = heading.Replace("E", "");
                heading = heading.Replace("W", "");

                heading = heading.Replace("R", "");
                heading = heading.Replace("C", "");
                heading = heading.Replace("L", "");
                heading = heading.Replace("U", "");

                heading = heading.Replace("G", "");
                heading = heading.Replace("X", "");

                runways = heading.Split('/');
            }

            String firstRunway = runways[0];

            if ((firstRunway == "") || (!Char.IsDigit(firstRunway[0])))
            {
                aptHeliport.WriteLine("\t<Placemark>");
                aptHeliport.WriteLine("\t\t<name>" + columns[0] + ":" + columns[5] + ":" + columns[7] + "</name>");
                aptHeliport.WriteLine("\t\t<Point>");

                aptHeliport.Write("\t\t\t<coordinates>");

                aptHeliport.Write(latLon.decimalLon.ToString("F6") + "," + latLon.decimalLat.ToString("F6"));

                aptHeliport.WriteLine("</coordinates>");

                aptHeliport.WriteLine("\t\t</Point>");
                aptHeliport.WriteLine("\t</Placemark>");
            }
            else
            {
                aptRunway.WriteLine("\t<Placemark>");
                aptRunway.WriteLine("\t\t<name>" + columns[5] + ":" + columns[7] + "</name>");

                if (columns[6] == "WATER")
                {
                    aptRunway.WriteLine("\t\t<styleUrl>#waterport</styleUrl>");
                }
                else if (columns[5].IndexOf('H') == 0)
                {
                    aptRunway.WriteLine("\t\t<styleUrl>#heliport</styleUrl>");
                }
                else
                {
                    aptRunway.WriteLine("\t\t<styleUrl>#airport</styleUrl>");
                }

                aptRunway.WriteLine("\t\t<LineString>");

                aptRunway.Write("\t\t\t<coordinates>");

                aptRunway.Write(latLon.decimalLon.ToString("F6") + "," + latLon.decimalLat.ToString("F6") + ",100");

                LatLon tolatLon = latLon.PointFromHeadingDistance((Convert.ToDouble(columns[7]) / 5280), Convert.ToDouble(runways[0] + "0") + (Convert.ToDouble(columns[4])));

                aptRunway.Write(" " + tolatLon.decimalLon.ToString("F6") + "," + tolatLon.decimalLat.ToString("F6") + ",100");

                aptRunway.WriteLine("</coordinates>");

                aptRunway.WriteLine("\t\t</LineString>");
                aptRunway.WriteLine("\t</Placemark>");
            }
        }


        private static void ProcessRow(String row)
        {
            columns = row.Split('~');

            if (prevICAO != columns[0])
            {
                wrotePoint = false;

                prevICAO = columns[0];
            }

            if (!wrotePoint)
            {
                wrotePoint = true;

                latLon = new LatLon(columns[2], columns[3]);

                if (columns[6] == "WATER")
                {
                    aptWaterport.WriteLine("\t<Placemark>");
                    aptWaterport.WriteLine("\t\t<name>" + columns[0] + ":" + columns[1].Replace("&", "&amp;") + "</name>");
                    aptWaterport.WriteLine("\t\t<Point>");

                    aptWaterport.Write("\t\t\t<coordinates>");

                    aptWaterport.Write(latLon.decimalLon.ToString("F6") + "," + latLon.decimalLat.ToString("F6"));

                    aptWaterport.WriteLine("</coordinates>");

                    aptWaterport.WriteLine("\t\t</Point>");
                    aptWaterport.WriteLine("\t</Placemark>");
                }
                else if (columns[5].IndexOf('H') == 0)
                {
                    aptHeliport.WriteLine("\t<Placemark>");
                    aptHeliport.WriteLine("\t\t<name>" + columns[0] + ":" + columns[1].Replace("&", "&amp;") + "</name>");
                    aptHeliport.WriteLine("\t\t<Point>");

                    aptHeliport.Write("\t\t\t<coordinates>");

                    aptHeliport.Write(latLon.decimalLon.ToString("F6") + "," + latLon.decimalLat.ToString("F6"));

                    aptHeliport.WriteLine("</coordinates>");

                    aptHeliport.WriteLine("\t\t</Point>");
                    aptHeliport.WriteLine("\t</Placemark>");
                }
                else
                {
                    aptAirport.WriteLine("\t<Placemark>");
                    aptAirport.WriteLine("\t\t<name>" + columns[0] + ":" + columns[1].Replace("&", "&amp;") + "</name>");

                    aptAirport.WriteLine("\t\t<Point>");

                    aptAirport.Write("\t\t\t<coordinates>");

                    aptAirport.Write(latLon.decimalLon.ToString("F6") + "," + latLon.decimalLat.ToString("F6"));

                    aptAirport.WriteLine("</coordinates>");

                    aptAirport.WriteLine("\t\t</Point>");
                    aptAirport.WriteLine("\t</Placemark>");
                }
            }

            WriteRunway();
        }


        private static void Main(string[] args)
        {
            rowset = new StreamReader(args[0], false);

            aptAirport.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>");
            aptAirport.WriteLine("<kml xmlns=\"http://www.opengis.net/kml/2.2\">");
            aptAirport.WriteLine("<Document>");

            aptWaterport.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>");
            aptWaterport.WriteLine("<kml xmlns=\"http://www.opengis.net/kml/2.2\">");
            aptWaterport.WriteLine("<Document>");

            aptHeliport.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>");
            aptHeliport.WriteLine("<kml xmlns=\"http://www.opengis.net/kml/2.2\">");
            aptHeliport.WriteLine("<Document>");

            aptRunway.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>");
            aptRunway.WriteLine("<kml xmlns=\"http://www.opengis.net/kml/2.2\">");
            aptRunway.WriteLine("<Document>");
            aptRunway.WriteLine("\t<Style id=\"airport\">");
            aptRunway.WriteLine("\t\t<LineStyle>");
            aptRunway.WriteLine("\t\t\t<color>FFB5E4FF</color>");
            aptRunway.WriteLine("\t\t\t<width>2</width>");
            aptRunway.WriteLine("\t\t</LineStyle>");
            aptRunway.WriteLine("\t</Style>");
            aptRunway.WriteLine("\t<Style id=\"heliport\">");
            aptRunway.WriteLine("\t\t<LineStyle>");
            aptRunway.WriteLine("\t\t\t<color>FF008CFF</color>");
            aptRunway.WriteLine("\t\t\t<width>2</width>");
            aptRunway.WriteLine("\t\t</LineStyle>");
            aptRunway.WriteLine("\t</Style>");
            aptRunway.WriteLine("\t<Style id=\"waterport\">");
            aptRunway.WriteLine("\t\t<LineStyle>");
            aptRunway.WriteLine("\t\t\t<color>FFD0E040</color>");
            aptRunway.WriteLine("\t\t\t<width>2</width>");
            aptRunway.WriteLine("\t\t</LineStyle>");
            aptRunway.WriteLine("\t</Style>");


            row = rowset.ReadLine();

            while (!rowset.EndOfStream)
            {
                ProcessRow(row);

                row = rowset.ReadLine();
            }

            ProcessRow(row);


            aptAirport.WriteLine("</Document>");
            aptAirport.WriteLine("</kml>");

            aptWaterport.WriteLine("</Document>");
            aptWaterport.WriteLine("</kml>");

            aptHeliport.WriteLine("</Document>");
            aptHeliport.WriteLine("</kml>");

            aptRunway.WriteLine("</Document>");
            aptRunway.WriteLine("</kml>");

            aptAirport.Close();
            aptWaterport.Close();
            aptHeliport.Close();
            aptRunway.Close();

            rowset.Close();
        }
    }
}
