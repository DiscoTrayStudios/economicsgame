using System.Collections;
using System.Collections.Generic;

public class Country
{
    public string Name { get; }
    public double GDP { set; get; }
    public double Growth { set; get; }
    public double Emissions { set; get; }
    public bool HaveAgreed { get; set; }
    public double PercentageOfTotalEmissions { get; set; }
    public double Score { get; set; }
    public bool PerfectAgree { get; set; }
    public bool PerfectDisagree { get; set; }

    public Country(string name)
    {
        Name = name;
        GDP = 20000;
        Growth = 0.2f;
        Emissions = 0.5f;
        HaveAgreed = false;
        PercentageOfTotalEmissions = 0.0f;
        PerfectAgree = true;
        PerfectDisagree = true;
    }

    public void adjustGDP(double d)
    {
        GDP += d;
    }

    public void adjustGrowth(double d)
    {
        Growth += d;
    }

    // "d" is a percentage by which to adjust emissions
    public void adjustEmissions(double d)
    {
        Emissions += (Emissions * d);
    }
    public void adjustScore(double eMulti)
	{
        Score = GDP - ((Emissions - 0.5) * (eMulti));
	}

    public void Agree()
    {
        HaveAgreed = true;
        if (PerfectDisagree) PerfectDisagree = false;
    }

    public void Decline()
    {
        HaveAgreed = false;
        if (PerfectAgree) PerfectAgree= false;
    }

    public void ActivateGDPGrowth()
    {
        double growth = Growth * GDP;
        adjustGDP(growth);
    }

    public void UpdatePercentageOfEmissions(double totalEmissions)
    {
        PercentageOfTotalEmissions = Emissions / totalEmissions;
    }
}
