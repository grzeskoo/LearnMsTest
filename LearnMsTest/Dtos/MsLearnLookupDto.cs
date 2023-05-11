using System.Collections.Generic;

namespace LearnMsTest.Dtos;

public class MsLearnLookupDto
{
    public string Id { get; set; }
    public string Name { get; set; }
    public IList<MsLearnLookupDto> Children { get; set; }
}