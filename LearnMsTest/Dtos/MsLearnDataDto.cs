using System.Collections.Generic;

namespace LearnMsTest.Dtos;

public class MsLearnDataDto
{
    public List<MsLearnDto> Modules { get; set; }
    public List<MsLearnDto> LearningPaths { get; set; }
    public List<MsLearnLookupDto> Levels { get; set; }
    public List<MsLearnLookupDto> Roles { get; set; }
    public List<MsLearnLookupDto> Products { get; set; }
}