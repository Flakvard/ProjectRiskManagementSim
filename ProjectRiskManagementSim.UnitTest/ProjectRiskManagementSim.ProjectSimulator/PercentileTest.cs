using ProjectRiskManagementSim.ProjectSimulation;

namespace ProjectRiskManagementSim.UnitTest;

public class ProgramTest
{
    [Fact]
    public void ProgramTestExtension()
    {

        // Arrange
        // 17.113380000000003
        var arr = new double[]{
          10.8362,
          6.1905,
          12.1807,
          4.6223,
          13.2322,
          11.9694,
          17.8705,
          16.5041,
          8.6809,
          10.8635,
          14.5008,
          14.3480,
          16.7889,
          15.0872,
          19.0232,
          14.7737,
          4.4629,
          2.3577
        };
        var emptyArr = new double[] { };
        var oneValArr = new double[] { 42 };

        // Act
        var percentile90 = arr.Percentile(0.9);
        var percentile50 = arr.Percentile(0.5);
        var percentile30 = arr.Percentile(0.3);
        var percentile0 = arr.Percentile(0);
        var percentile100 = arr.Percentile(1);


        // Assert with 5 decimal precision
        Assert.Equal(17.11338, percentile90, precision: 5);
        Assert.Equal(12.70645, percentile50, precision: 5);
        Assert.Equal(10.83893, percentile30, precision: 5);
        Assert.Equal(2.35770, percentile0, precision: 5);
        Assert.Equal(19.02320, percentile100, precision: 5);

        // Empty Array
        Assert.Throws<InvalidOperationException>(() => emptyArr.Percentile(0.5));

        // One value Array
        Assert.Equal(42, oneValArr.Percentile(0));  // Min
        Assert.Equal(42, oneValArr.Percentile(0.5));  // Median
        Assert.Equal(42, oneValArr.Percentile(1));  // Max

        // Over 0..1 percentile
        Assert.Throws<ArgumentOutOfRangeException>(() => arr.Percentile(-0.1));
        Assert.Throws<ArgumentOutOfRangeException>(() => arr.Percentile(1.1));
    }
}
