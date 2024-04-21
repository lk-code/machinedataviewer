using DataViewer.Core;
using DataViewer.Core.Contracts;
using FluentAssertions;

namespace DataViewer.Tests.Core;

[TestClass]
public class MachineDataExtractorTests
{
    private IDataExtractor _instance = null!;

    [TestInitialize]
    public void SetUp()
    {
        _instance = new MachineDataExtractor();
    }

    [TestMethod]
    public async Task GetPortsFromData_WithDefaultTestData_Returns()
    {
        var testData = """
            www.micro-epsilon.com
            scanCONTROL Configuration Tools

            Aktive Programme:
            Programm 1: Erster Punkt
            Programm 2: Offset
            Programm 3: Erster Punkt
            Programm 4: Letzter Punkt
            Programm 5: Spalt: Abstand X/Z

            scanCONTROL 2910-10 /BLv46-15
            SN: 219040037
            Profilfrequenz [1/s]: 25
            Zuletzt verwendete Parameter: User mode 3

            Toleranzen für Digitalausgänge (Untere/Obere Grenze)
            Port 1: Kombiniert: Kopfbreite: 0.000 / 0.000
            Port 2: Programm 5: Abstand X: 0.000 / 0.000
            Port 3: Kein: 0.000 / 0.000
            Port 4: Kein: 0.000 / 0.000
            Port 5: Kein: 0.000 / 0.000
            Port 6: Kein: 0.000 / 0.000
            Port 7: Kein: 0.000 / 0.000
            Port 8: Kein: 0.000 / 0.000

            Date	    Time	Profilnr.	Kombiniert: Kopfbreite [mm]	Programm 5: Abstand X [mm]	
            04/11/2023	09:40:09:630	6616731	0.522	0.063	
            04/11/2023	09:40:09:662	6616732	0.522	0.069	
            04/11/2023	09:40:09:695	6616733	0.522	0.069	
            04/11/2023	09:40:09:742	6616734	0.522	0.075	
            """;

        var result = (await _instance.GetPortsFromDataAsync(testData, CancellationToken.None))
            .ToList();

        result.Should().NotBeNullOrEmpty();
        result.Count().Should().Be(8);
        result[0].Should().Be("Port 1: Kombiniert: Kopfbreite: 0.000 / 0.000");
        result[1].Should().Be("Port 2: Programm 5: Abstand X: 0.000 / 0.000");
        result[2].Should().Be("Port 3: Kein: 0.000 / 0.000");
        result[3].Should().Be("Port 4: Kein: 0.000 / 0.000");
        result[4].Should().Be("Port 5: Kein: 0.000 / 0.000");
        result[5].Should().Be("Port 6: Kein: 0.000 / 0.000");
        result[6].Should().Be("Port 7: Kein: 0.000 / 0.000");
        result[7].Should().Be("Port 8: Kein: 0.000 / 0.000");
    }

    [TestMethod]
    public async Task GetAnalyticsFromData_WithDefaultTestData_Returns()
    {
        var testData = """
            www.micro-epsilon.com
            scanCONTROL Configuration Tools

            Aktive Programme:
            Programm 1: Erster Punkt
            Programm 2: Offset
            Programm 3: Erster Punkt
            Programm 4: Letzter Punkt
            Programm 5: Spalt: Abstand X/Z

            scanCONTROL 2910-10 /BLv46-15
            SN: 219040037
            Profilfrequenz [1/s]: 25
            Zuletzt verwendete Parameter: User mode 3

            Toleranzen für Digitalausgänge (Untere/Obere Grenze)
            Port 1: Kombiniert: Kopfbreite: 0.000 / 0.000
            Port 2: Programm 5: Abstand X: 0.000 / 0.000
            Port 3: Kein: 0.000 / 0.000
            Port 4: Kein: 0.000 / 0.000
            Port 5: Kein: 0.000 / 0.000
            Port 6: Kein: 0.000 / 0.000
            Port 7: Kein: 0.000 / 0.000
            Port 8: Kein: 0.000 / 0.000

            Date	    Time	Profilnr.	Kombiniert: Kopfbreite [mm]	Programm 5: Abstand X [mm]	
            04/11/2023	09:40:09:630	6616731	0.522	0.063	
            04/11/2023	09:40:09:662	6616732	0.522	0.069	
            04/11/2023	09:40:09:695	6616733	0.522	0.069	
            04/11/2023	09:40:09:742	6616734	0.522	0.075	
            """;

        var result = (await _instance.GetAnalyticsFromDataAsync(testData, CancellationToken.None))
            .ToList();

        result.Should().NotBeNullOrEmpty();
        result.Count().Should().Be(4);
        result[0].DateValue.Should().Be("04/11/2023");
        result[0].TimeValue.Should().Be("09:40:09:630");
        result[0].DateTime.Should().Be(new DateTime(2023, 4, 11, 9, 40, 9, 630));
        result[0].ProfileNumber.Should().Be(6616731);
        result[0].HeadWidth.Should().Be(0.522m);
        result[0].Distance.Should().Be(0.063m);
    }
}
