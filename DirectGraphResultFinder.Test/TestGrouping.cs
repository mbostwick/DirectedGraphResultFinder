using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace DirectGraphResultFinder.Test
{
    using Model;

    [TestClass]
    public class TestGrouping
    {
        private const int min_site_name_length = 8;
        private const int max_site_name_length = 9;

        private const int min_number_of_regions = 3;
        private const int max_number_of_regions = 5;

        private const int min_number_of_sites_in_region = 5;
        private const int max_number_of_sites_in_region = 6;

        private const int min_number_of_mesh_items = 2;
        private const int max_number_of_mesh_items = 4;

        private const int min_number_of_mesh_connections = 2;
        private const int max_number_of_mesh_connections = 5;
        

        private static Dictionary<string, bool> usedSiteNames { get; set; }

        private static Random randomActionHolder { get; set; }

        private static int test_seed {get;set;}

        private static Random getRandomHandle()
        {
            if (randomActionHolder == null)
            {
                if (test_seed == 0)
                {
                    randomActionHolder = new Random();
                }
                else
                {
                    randomActionHolder = new Random(test_seed);
                }
            }
            return randomActionHolder;
        }

        [TestMethod]
        public void TestRandomSiteNames()
        {
            var getRandom = getRandomHandle();
            var numberOfSites = getRandom.Next(0,32);
            var siteNames = getSiteNames(numberOfSites);
            Assert.AreEqual(numberOfSites, siteNames.Length);
        }

        [TestMethod]
        public void TestLinkTextMapping()
        {
            test_seed = 100;
            var generatedRegion = getRandomRegion();
            var linksForRegion = generatedRegion.links;
            var givenText = ProcessData.exportInformation(linksForRegion);
            var regionDataForText = ProcessData.parseGivenInput(givenText);
            var generatedMd5 = getMd5SumForLinks(linksForRegion);
            var methodMd5 = getMd5SumForLinks(regionDataForText);
            Assert.AreEqual(methodMd5, generatedMd5);
        }

        [TestMethod]
        public void TestRegionGrouping()
        {
            test_seed = 100;
            var generatedRegions = getRandomRegionData();
            var regionsResolved = ProcessData.findRegions(pullLinkDataFromRegionData(generatedRegions),null);
            var resultOfComparingRegionData = areRegionsEqual(generatedRegions, regionsResolved);
            Assert.AreEqual(true, resultOfComparingRegionData);
        }

        private static bool areRegionsEqual(DataPointRegion[] regionDataA,DataPointRegion[] regionDataB )
        {
            var sitesAreEqual = true;
            var md5SumsForRegionDataA = new List<string>();
            foreach (var region in regionDataA)
            {
                md5SumsForRegionDataA.Add(getMd5SumForRegion(region));
            }
            var md5SumsForRegionDataB = new List<string>();
            foreach (var region in regionDataB)
            {
                md5SumsForRegionDataB.Add(getMd5SumForRegion(region));
            }
            sitesAreEqual = (md5SumsForRegionDataA.Count == md5SumsForRegionDataB.Count);
            if (sitesAreEqual)
            {
                md5SumsForRegionDataA.Sort();
                md5SumsForRegionDataB.Sort();
                for (int currentRegionSearch = 0; currentRegionSearch < md5SumsForRegionDataA.Count; currentRegionSearch++)
                {
                    if (!md5SumsForRegionDataA[currentRegionSearch].Equals(md5SumsForRegionDataB[currentRegionSearch]))
                    {
                        sitesAreEqual = false;
                        break;
                    }
                }
            }
            return sitesAreEqual;
        }

        private static string getMd5SumForRegion(DataPointRegion regionToGetMd5SumFor)
        {
            return getMd5SumForLinks(regionToGetMd5SumFor.links);
        }
        private static string getMd5SumForLinks(LinkedData[] givenLinks)
        {
            var siteNamesForRegion = new List<string>();
            foreach (var link in givenLinks)
            {
                siteNamesForRegion.Add(link.pointA.name);
                siteNamesForRegion.Add(link.pointB.name);
            }
            siteNamesForRegion.Sort();

            var textBuilder = new StringBuilder();
            foreach (var site in siteNamesForRegion)
            {
                textBuilder.Append(site);
                textBuilder.Append(LinkedData.link_line_separator);
            }
            return calculateMD5Hash(textBuilder.ToString());
        }

        private static LinkedData[] pullLinkDataFromRegionData(DataPointRegion[] regionData)
        {
            var linkDataFromRegions = new List<LinkedData>();
            foreach (var region in regionData)
            {
                linkDataFromRegions.AddRange(region.links);
            }
            for (int linkPossition = 0; linkPossition < linkDataFromRegions.Count; linkPossition++)
            {
                linkDataFromRegions[linkPossition].edge_id = linkPossition;
            }
            return linkDataFromRegions.ToArray();
        }

        private static DataPointRegion[] getRandomRegionData()
        {
            var getRandomInformation = getRandomHandle();
            var numberOfRegions = getRandomInformation.Next(min_number_of_regions, max_number_of_regions);
            var regionsToReturn = new DataPointRegion[numberOfRegions];
            int currentFilterCount = 1;
            for (int regionPossition = 0; regionPossition < numberOfRegions; regionPossition++)
            {
                regionsToReturn[regionPossition] = getRandomRegion(currentFilterCount);
                currentFilterCount += regionsToReturn[regionPossition].links.Length;
            }
            return regionsToReturn;
        }

        private static DataPointRegion getRandomRegion()
        {
            return getRandomRegion(1);
        }

        private static DataPointRegion getRandomRegion(int baseToUseForUniqueEdgeIds)
        {
            var getRandomInformation = getRandomHandle();
            var totalSitesInRegion = getRandomInformation.Next(min_number_of_sites_in_region, max_number_of_sites_in_region);
            var itemsInMesh = getRandomInformation.Next(min_number_of_mesh_items, max_number_of_mesh_items);
            itemsInMesh = itemsInMesh > totalSitesInRegion ? totalSitesInRegion : itemsInMesh;
            itemsInMesh = itemsInMesh == 1 ? 2 : itemsInMesh;
            var itemsSetupAsSpur = totalSitesInRegion - itemsInMesh;
            var disconnectedSiteListForRegion = getSiteNames(totalSitesInRegion).ToList();
            var linkInfo = new List<LinkedData>();
            if (itemsInMesh > 0)
            {
                linkInfo.AddRange(getRandomMeshConnections(itemsInMesh, ref disconnectedSiteListForRegion));
            }
            if (itemsSetupAsSpur > 0)
            {
                linkInfo.AddRange(getRandomMostlySpurConnections(ref linkInfo, ref disconnectedSiteListForRegion));
            }
            linkInfo.AddRange(getLinksToEnsureSiteCanReachAllSites(ref linkInfo));
            linkInfo = simplifyLinks(ref linkInfo);
            for (int linkPossition = 0; linkPossition < linkInfo.Count; linkPossition++)
            {
                var numberForLink = baseToUseForUniqueEdgeIds + linkPossition;
                linkInfo[linkPossition].edge_id = numberForLink;
            }
            return new DataPointRegion(baseToUseForUniqueEdgeIds.ToString(),linkInfo.ToArray());
        }

        private static List<LinkedData> simplifyLinks(ref List<LinkedData> existingLinks)
        {
            var linksWithOutDuplicates = new List<LinkedData>();
            var linkedSites = new Dictionary<string, List<string>>();
            var sitesForLocation = findWhatSitesConnectTo(ref existingLinks);
            var sitesToLink = sitesForLocation.Keys.ToArray();
            foreach (var site in sitesToLink)
            {
                var connectedSites = sitesForLocation.Keys.ToArray();
                for (int connectedSite = (connectedSites.Length - 1); connectedSite >= 0; connectedSite--)
                {
                    if (!site.Equals(connectedSites[connectedSite]))
                    {
                        var sitesForLink = new List<string> { site, connectedSites[connectedSite] };
                        sitesForLink.Sort();
                        if (linkedSites.ContainsKey(sitesForLink[0]))
                        {
                            if (!linkedSites[sitesForLink[0]].Contains(sitesForLink[1]))
                            {
                                linkedSites[sitesForLink[0]].Add(sitesForLink[1]);
                                linksWithOutDuplicates.Add(new LinkedData(new DataPoint(sitesForLink[0]), new DataPoint(sitesForLink[1])));
                            }
                        }
                        else
                        {
                            linkedSites.Add(sitesForLink[0], new List<string> { sitesForLink[1] });
                            linksWithOutDuplicates.Add(new LinkedData(new DataPoint(sitesForLink[0]), new DataPoint(sitesForLink[1])));
                        }
                    }
                }
            }
            return linksWithOutDuplicates;
        }

        private static LinkedData[] getRandomMeshConnections(int numberOfSitesToMesh,ref List<string> disconnectedSites)
        {
            var linksToReturn = new List<LinkedData>();
            var getRandomInformation = getRandomHandle();
            var sitesToMesh = new List<string>();
            if (numberOfSitesToMesh < 2) throw new Exception("At least 2 sites must be allowed in order to mesh!");
            for (int sitesToHandle = (numberOfSitesToMesh-1); sitesToHandle >=0; sitesToHandle--)
            {
                var siteToMesh = disconnectedSites[sitesToHandle];
                sitesToMesh.Add(siteToMesh);
                disconnectedSites.Remove(siteToMesh);
            }
            foreach (var siteToMesh in sitesToMesh)
            {
                var numberOfMeshLinks = getRandomInformation.Next(min_number_of_mesh_connections, max_number_of_mesh_connections);
                for (int currentMeshLink = 0; currentMeshLink < numberOfMeshLinks; currentMeshLink++)
                {
                    var secondMeshPoints = siteToMesh;
                    while (secondMeshPoints.Equals(siteToMesh))
                    {
                        var possitionToLinkWith = (getRandomInformation.Next(0, sitesToMesh.Count));
                        secondMeshPoints = sitesToMesh[possitionToLinkWith];
                    }
                    linksToReturn.Add(new LinkedData(new DataPoint(siteToMesh), new DataPoint(secondMeshPoints)));
                }
            }
            linksToReturn.AddRange(getConnectionsNeededForMesh(ref linksToReturn, sitesToMesh));
            return linksToReturn.ToArray();
        }

        private static LinkedData[] getConnectionsNeededForMesh(ref List<LinkedData> existingLinks, List<string> allSites)
        {
            var linksToMakeFullMesh = new List<LinkedData>();
            var sitesConnected = findWhatSitesConnectTo(ref existingLinks);
            foreach (var siteConnection in sitesConnected)
            {
                foreach (var sitesToConfirmMeshed in allSites)
                {
                    if(!siteConnection.Value.Contains(sitesToConfirmMeshed))
                    {
                        linksToMakeFullMesh.Add(new LinkedData(new DataPoint(siteConnection.Key), new DataPoint(sitesToConfirmMeshed)));
                    }
                }
            }
            return linksToMakeFullMesh.ToArray();
        }

        private static LinkedData[] getLinksToEnsureSiteCanReachAllSites(ref List<LinkedData> existingLinks)
        {
            return getLinksToEnsureSiteCanReachAllSites(existingLinks[0].pointA.name,ref existingLinks);
        }
        private static LinkedData[] getLinksToEnsureSiteCanReachAllSites(string siteToConfirmCanReachAllSites,ref List<LinkedData> existingLinks)
        {
            var missingLinksForSite = new List<LinkedData>();
            var sitesFromLinks = findWhatSitesConnectTo(ref existingLinks);
            var allSites = sitesFromLinks.Keys.ToArray();
            var currentConnections = sitesFromLinks[siteToConfirmCanReachAllSites];
            foreach (var site in allSites)
            {
                if (!currentConnections.Contains(site))
                {
                    missingLinksForSite.Add(new LinkedData(new DataPoint(siteToConfirmCanReachAllSites), new DataPoint(site)));
                }
            }
            return missingLinksForSite.ToArray();
        }

        private static Dictionary<string, List<string>> findWhatSitesConnectTo(ref List<LinkedData> givenDataPoints)
        {
            var siteWIthWhatItsConnectedTo = new Dictionary<string, List<string>>();
            foreach (var link in givenDataPoints)
            {
                if (siteWIthWhatItsConnectedTo.ContainsKey(link.pointA.name))
                {
                    siteWIthWhatItsConnectedTo[link.pointA.name].Add(link.pointB.name);
                }
                else
                {
                    siteWIthWhatItsConnectedTo.Add(link.pointA.name, new List<string> { link.pointB.name });
                }
                if (siteWIthWhatItsConnectedTo.ContainsKey(link.pointB.name))
                {
                    siteWIthWhatItsConnectedTo[link.pointB.name].Add(link.pointA.name);
                }
                else
                {
                    siteWIthWhatItsConnectedTo.Add(link.pointB.name, new List<string> { link.pointA.name });
                }
            }
            return siteWIthWhatItsConnectedTo;
        }

        private static LinkedData[] getRandomMostlySpurConnections(ref List<LinkedData> existingLinks,ref List<string> disconnectedSites)
        {
            var linksToReturn = new LinkedData[disconnectedSites.Count];
            var getRandomInformation = getRandomHandle();
            for (int currentSpurPos = (disconnectedSites.Count - 1); currentSpurPos >= 0; currentSpurPos--)
            {
                var firstConnectionPoint = new DataPoint();
                var secondConnectionPoint = new DataPoint(disconnectedSites[currentSpurPos]);
                disconnectedSites.RemoveAt(currentSpurPos);
                var connectToExistingPointA = (getRandomInformation.Next(0, 1) > 0);
                if (existingLinks.Count == 0)
                {
                    var existingLinkToUse = existingLinks[getRandomInformation.Next(0, (existingLinks.Count - 1))];
                    firstConnectionPoint.name = connectToExistingPointA  ? existingLinkToUse.pointA.name:existingLinkToUse.pointB.name;
                    firstConnectionPoint.name = currentSpurPos == 0 ? linksToReturn[linksToReturn.Length - 1].pointB.name : disconnectedSites[0];
                }
                else
                {
                    var existingLinkToUse = existingLinks[getRandomInformation.Next(0, (existingLinks.Count - 1))];
                    firstConnectionPoint.name = connectToExistingPointA  ? existingLinkToUse.pointA.name:existingLinkToUse.pointB.name;
                }
                linksToReturn[currentSpurPos] = new LinkedData(firstConnectionPoint, secondConnectionPoint);

            }
            return linksToReturn;
        }

        private static string[] getSiteNames(int numberOfSites)
        {
            var siteNames = new string[numberOfSites];
            for (int sitePosition = 0; sitePosition < numberOfSites; sitePosition++)
            {
                siteNames[sitePosition] = getRandomSiteName();
            }
            return siteNames;
        }

        private static string getRandomSiteName()
        {
            if (usedSiteNames == null)
            {
                usedSiteNames = new Dictionary<string, bool>();
            }
            var getRandomInformation = getRandomHandle();
            var siteNameString = String.Empty;
            while (String.IsNullOrEmpty(siteNameString) || usedSiteNames.ContainsKey(siteNameString))
            {
                var siteNameLength = getRandomInformation.Next(min_site_name_length, max_site_name_length);
                var siteNmae = new char[siteNameLength];
                for (int siteLetterPos = 0; siteLetterPos < siteNameLength; siteLetterPos++)
                {
                    siteNmae[siteLetterPos] = (char)('a' + getRandomInformation.Next(0, 26));
                }
                siteNameString = new string(siteNmae);
            }
            usedSiteNames.Add(siteNameString, true);
            return siteNameString;
        }

        private static string calculateMD5Hash(string input)
        {
            // step 1, calculate MD5 hash from input
            var md5 = System.Security.Cryptography.MD5.Create();
            var inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            var hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string
            var sb = new StringBuilder();
            for (int charInHash = 0; charInHash < hash.Length; charInHash++)
            {
                sb.Append(hash[charInHash].ToString("X2"));
            }
            return sb.ToString();
        }

    }
}
