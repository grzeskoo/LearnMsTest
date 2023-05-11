using System.Collections.Generic;
using System;

namespace LearnMsTest.Dtos;

public class MsLearnDto
{
    public string Summary { get; set; }
    public List<string> Levels { get; set; }
    public List<string> Roles { get; set; }
    public List<string> Products { get; set; }
    public string Uid { get; set; }
    public string Type { get; set; }
    public string Title { get; set; }
    public int? Duration_In_Minutes { get; set; }
    public MsLearnRatingDto Rating { get; set; }
    public string Icon_Url { get; set; }
    public string Locale { get; set; }
    public DateTime? Last_Modified { get; set; }
    public string Url { get; set; }
    public int? Number_Of_Children { get; set; }
    public double? Popularity { get; set; }
    public string FirstUnitUrl { get; set; }
    public List<string> Modules { get; set; }
}