using Kesco.Lib.BaseExtention.Enums.Docs;

namespace Kesco.Lib.Entities.Documents.EF.MyDocuments
{
	public class MyDocument : Document
	{
		public DocField MyField { get; private set; }
		public MyDocument()
		{
			Type = DocTypeEnum.СчетФактура;
			MyField = GetDocField("75");//Продавец
		}
	}
}