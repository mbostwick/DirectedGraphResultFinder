using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace DirectGraphResultFinder
{
    using Model;

    public static class ProcessData
    {
        private const double change_ratio_to_use_for_reporting = 5;
        private const int column_possition_for_edge_id = 3;

        private static string invalid_input_given_for_parsing = "Input data can not be empty, it must be a path or data containing 2 to 3 columns separated by " + LinkedData.link_line_separator + "and " + LinkedData.datapoint_column_separator + " to delineate links(linkPointA,linkPointB,[linkEdgeID])!";
        private const string invalid_edge_id_status = "Input data must have the third column be a number representing the edge id, or must be left empty for the entire set of the data(in which case the edge id will be generated)!";

        public static string exportInformation(string givenInput)
        {
            return exportInformation(findRegions(parseGivenInput(givenInput),null));
        }

        public static LinkedData[] parseGivenInput(string inputGiven)
        {
            if (String.IsNullOrEmpty(inputGiven)) throw new ArgumentNullException(invalid_input_given_for_parsing);
            var textToParse = inputGiven;
            if (File.Exists(inputGiven))
            {
                textToParse = File.ReadAllText(inputGiven);
            }
            var expectColumnForNumber = false;
            var arrayHoldingLines = textToParse.Split(LinkedData.link_line_separator);
            var pointsToReturn = new LinkedData[arrayHoldingLines.Length];
            for (int linePosInData = 0; linePosInData < arrayHoldingLines.Length; linePosInData++)
            {
                var columnsFromLine = arrayHoldingLines[linePosInData].Split(LinkedData.datapoint_column_separator);
                if (columnsFromLine.Length > 3 || columnsFromLine.Length < 2) throw new ArgumentException(invalid_input_given_for_parsing);
                int edgeId = linePosInData;
                if (linePosInData == 0)
                {
                    expectColumnForNumber = (columnsFromLine.Length == column_possition_for_edge_id);
                }
                if (expectColumnForNumber)
                {
                    if (columnsFromLine.Length < column_possition_for_edge_id || !int.TryParse(columnsFromLine[2],out edgeId))
                    {
                        throw new ArgumentException(invalid_edge_id_status);
                    }
                }
                pointsToReturn[linePosInData] = new LinkedData(edgeId, new DataPoint(columnsFromLine[0]), new DataPoint(columnsFromLine[1]));
            }
           return pointsToReturn;
        }

        public static string exportInformation(DataPointRegion[] regionInformation)
        {
            var stringMaker = new StringBuilder();
            foreach (var region in regionInformation)
            {
                foreach (var link in region.links)
                {
                    stringMaker.Append(link.pointA.name);
                    stringMaker.Append(LinkedData.datapoint_column_separator);
                    stringMaker.Append(link.pointB.name);
                    stringMaker.Append(LinkedData.datapoint_column_separator);
                    stringMaker.Append(link.edge_id);
                    stringMaker.Append(LinkedData.datapoint_column_separator);
                    stringMaker.Append(region.name);
                    stringMaker.Append(LinkedData.link_line_separator);
                }
            }
            stringMaker.Remove(stringMaker.Length - 1,1);
            return stringMaker.ToString();
        }
        public static string exportInformation(LinkedData[] linkInformation)
        {
            var stringMaker = new StringBuilder();
            foreach (var link in linkInformation)
            {
                stringMaker.Append(link.pointA.name);
                stringMaker.Append(LinkedData.datapoint_column_separator);
                stringMaker.Append(link.pointB.name);
                stringMaker.Append(LinkedData.datapoint_column_separator);
                stringMaker.Append(link.edge_id);
                stringMaker.Append(LinkedData.link_line_separator);
            }
            stringMaker.Remove(stringMaker.Length - 1, 1);
            return stringMaker.ToString();
        }

        public static DataPointRegion[] findRegions(LinkedData[] givenDataPoints,progressChanged methodForReportingProgress)
        {
            int maxEdge =Int32.MinValue;
            int minEdge = Int32.MaxValue;
            foreach (var link in givenDataPoints)
            {
                if (link.edge_id > maxEdge)
                {
                    maxEdge = link.edge_id;
                }
                if (link.edge_id < minEdge)
                {
                    minEdge = link.edge_id;
                }
            }
            var edgeToPoints = linkedDataToEdges(givenDataPoints);
            var itemsLeftToHandle = givenDataPoints.ToList();
            var regionGrouping =  new Dictionary<int, List<LinkedData>>();
            var excludedLinks = new List<int>();
            double lastReportedProgress = 0;

            
            for (int currentGraphLink = minEdge; currentGraphLink <= maxEdge; currentGraphLink++)
            {
                if (!excludedLinks.Contains(currentGraphLink))
                {
                    int searchingLocation = currentGraphLink;
                    var connectedItems = getConnectedItems(ref edgeToPoints, ref itemsLeftToHandle, searchingLocation, maxEdge);
                    if (connectedItems.Count > 0)
                    {
                        regionGrouping.Add(currentGraphLink, connectedItems);
                        foreach (var link in connectedItems)
                        {
                            excludedLinks.Add(link.edge_id);
                        }
                    }
                }
                if (currentGraphLink > 0)
                {
                    double currentProgress = maxEdge / currentGraphLink;
                    if ((lastReportedProgress - currentProgress) > change_ratio_to_use_for_reporting)
                    {
                        if (methodForReportingProgress != null)
                        {
                            methodForReportingProgress.Invoke(Convert.ToInt32(Math.Round(currentProgress, 0)));
                        }
                    }
                }
            }
            return buildRegionArraryFromPointInformation(regionGrouping);
        }
        private static DataPointRegion[] buildRegionArraryFromPointInformation(Dictionary<int, List<LinkedData>> pointInformation)
        {
            var regionKeys = pointInformation.Keys.ToArray();
            var regionsToReturn = new DataPointRegion[regionKeys.Length];
            for (int possitionInPointInfo = 0; possitionInPointInfo < regionKeys.Length;possitionInPointInfo++)
            {
                regionsToReturn[possitionInPointInfo] = new DataPointRegion(regionKeys[possitionInPointInfo].ToString(), pointInformation[regionKeys[possitionInPointInfo]].ToArray());
            }
            return regionsToReturn;
        }

        private static Dictionary<int, LinkedData> linkedDataToEdges(LinkedData[] givenDataPoints)
        {
            var edgeMapping = new Dictionary<int, LinkedData>();
            for(int linkPossition = 0; linkPossition < givenDataPoints.Length;linkPossition++)
            {
                if (edgeMapping.ContainsKey(givenDataPoints[linkPossition].edge_id))
                {
                    throw new InvalidOperationException("Duplicate edge ID's in data (Duplicate Trigger:" + givenDataPoints[linkPossition].edge_id.ToString() + ")");
                }
                edgeMapping.Add(givenDataPoints[linkPossition].edge_id, givenDataPoints[linkPossition]);
            }
            return edgeMapping;
        }

        private static Dictionary<string, List<int>> getLinksForLocations(LinkedData[] givenDataPoints)
        {
            var linksForLocation = new Dictionary<string, List<int>>();
            foreach (var link in givenDataPoints)
            {
                if (linksForLocation.ContainsKey(link.pointA.name))
                {
                    if (!linksForLocation[link.pointA.name].Contains(link.edge_id))
                    {
                        linksForLocation[link.pointA.name].Add(link.edge_id);
                    }
                }
                else
                {
                    linksForLocation.Add(link.pointA.name, new List<int> { link.edge_id });
                }
                if (linksForLocation.ContainsKey(link.pointB.name))
                {
                    if (!linksForLocation[link.pointB.name].Contains(link.edge_id))
                    {
                        linksForLocation[link.pointB.name].Add(link.edge_id);
                    }
                }
                else
                {
                    linksForLocation.Add(link.pointB.name, new List<int> { link.edge_id });
                }
            }
            return linksForLocation;
        }

        private static List<LinkedData> getConnectedItems(ref Dictionary<int, LinkedData> linkIds,ref List<LinkedData> itemsLeftToHandle, int currentLink, int maxSearchLocation)
        {
            var connectionsForLocation = new List<LinkedData>();
            connectionsForLocation.Add(linkIds[currentLink]);
            var connectedSites = new List<string>() { connectionsForLocation[0].pointA.name, connectionsForLocation[0].pointB.name };
            var loopedThroughFindingNothing = true;
            while(loopedThroughFindingNothing)
            {
                loopedThroughFindingNothing = false;
                for (int locationSearchPossition = currentLink+1; locationSearchPossition <= maxSearchLocation; locationSearchPossition++)
                {
                    var currentPositionIsConnected = 
                        connectedSites.Contains(linkIds[locationSearchPossition].pointA.name) ||
                        connectedSites.Contains(linkIds[locationSearchPossition].pointB.name);
                    if (currentPositionIsConnected)
                    {
                        var linkFoundToBeConnected = linkIds[locationSearchPossition];
                        if (itemsLeftToHandle.Contains(linkFoundToBeConnected))
                        {
                            itemsLeftToHandle.Remove(linkFoundToBeConnected);
                            connectionsForLocation.Add(linkFoundToBeConnected);
                            loopedThroughFindingNothing = true;
                            if (!connectedSites.Contains(linkFoundToBeConnected.pointA.name))
                            {
                                connectedSites.Add(linkFoundToBeConnected.pointA.name);
                            }
                            if (!connectedSites.Contains(linkFoundToBeConnected.pointB.name))
                            {
                                connectedSites.Add(linkFoundToBeConnected.pointB.name);
                            }
                        }
                    }
                }
            }

            return connectionsForLocation;
        }

        public delegate void progressChanged(int currentProgress);
    }
}
