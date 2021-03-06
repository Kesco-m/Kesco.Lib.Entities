﻿using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using Kesco.Lib.BaseExtention.Enums;
using Kesco.Lib.BaseExtention.Enums.Docs;
using Kesco.Lib.Entities.Corporate;
using Kesco.Lib.Entities.Persons.PersonOld;
using Kesco.Lib.Web.Settings;

namespace Kesco.Lib.Entities.Documents.EF.Applications
{
    /// <summary>
    ///     Класс заявления на отпуск
    /// </summary>
    [Serializable]
    public class Vacation : Document
    {
        private PersonOld objectCompanyFrom;
        private Employee objectEmployeeFrom;
        private PersonOld objectEmployeeTo;
        private VacationType objectVacationType;

        /// <summary>
        /// </summary>
        public Vacation()
        {
            Initialization();
        }

        /// <summary>
        /// </summary>
        /// <param name="e"></param>
        public Vacation(Employee e)
        {
            User = e;
            Initialization();
        }

        /// <summary>
        /// </summary>
        /// <param name="id"></param>
        public Vacation(string id)
        {
            Id = id;
            LoadDocument(id, true);
            Initialization();
        }


        /// <summary>
        ///     Вид отпуска
        /// </summary>
        private VacationType ObjectVacationType
        {
            get
            {
                if (VacationTypeField.ValueString.Length == 0) return null;

                return objectVacationType ?? (objectVacationType = new VacationType(VacationTypeField.ValueString));
            }
            set { objectVacationType = value; }
        }

        /// <summary>
        ///     Сотрудник
        /// </summary>
        private Employee ObjectEmployeeFrom
        {
            get
            {
                if (EmployeeFromField.ValueString.Length == 0) return null;

                return objectEmployeeFrom ?? (objectEmployeeFrom = new Employee(EmployeeFromField.ValueString));
            }
            set { objectEmployeeFrom = value; }
        }


        /// <summary>
        ///     Лицо руководителя
        /// </summary>
        private PersonOld ObjectEmployeeTo
        {
            get
            {
                if (EmployeeToField.ValueString.Length == 0) return null;

                return objectEmployeeTo ?? (objectEmployeeTo = new PersonOld(EmployeeToField.ValueString));
            }
            set { objectEmployeeTo = value; }
        }

        /// <summary>
        ///     Лицо работодателя
        /// </summary>
        private PersonOld ObjectCompanyFrom
        {
            get
            {
                if (CompanyFromField.ValueString.Length == 0) return null;

                return objectCompanyFrom ?? (objectCompanyFrom = new PersonOld(CompanyFromField.ValueString));
            }
            set { objectCompanyFrom = value; }
        }


        //Дата конца отпуска и дата выхода на работу отличаются
        private Employee User { get; }

        /// <summary>
        ///     Инициализация документа Заявление на отпуск
        /// </summary>
        private void Initialization()
        {
            Type = DocTypeEnum.ЗаявлениеНаОтпуск;

            //Код лица сотрудника, от имени которого составляется заявление
            PersonFromField = GetDocField("1479");
            //Код сотрудника, от имени которого составляется заявление
            EmployeeFromField = GetDocField("1277");
            //Код лица компании сотрудника, от имени которого составляется заявление
            CompanyFromField = GetDocField("1275");
            //Код лица сотрудника, на имя которого составляется заявление
            EmployeeToField = GetDocField("1276");
            //Тип отпуска
            VacationTypeField = GetDocField("1280");
            //Дата начала отпуска
            DateFromField = GetDocField("1278");
            //Продолжительность отпуска в днях
            DaysField = GetDocField("1442");
            //Дата конца отпуска
            DateToField = GetDocField("1279");

            var doc_id = 0;
            int.TryParse(Id, out doc_id);
            if (0 == doc_id && null != User)
            {
                PersonFromField.Value = User.PersonEmployeeId;
                EmployeeFromField.Value = User.EmployeeId;
                if (User.Employer != null)
                    CompanyFromField.Value = User.Employer.Id;
            }
        }


        //TODO: Переделать/оптимизировать все, что ниже

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public DataTable GetPostSupervisor()
        {
            var dt = new DataTable();
            if (EmployeeFromField.ValueString.Length == 0) return dt;
            var sql = @"
DECLARE @Сотрудник nvarchar(300), @ДолжностьСотрудника nvarchar(300),
	@LСотрудника int, @RСотрудника int,
	@КодЛицаРуководителя int, @КодСотрудникаРуководителя int, @Руководитель nvarchar(300), @ДолжностьРуководителя nvarchar(300)

SELECT	 @Сотрудник=Я.Сотрудник,
		@ДолжностьСотрудника=МояДолжн.Должность,
		@LСотрудника=МояДолжн.L,
		@RСотрудника=МояДолжн.R,
		@КодСотрудникаРуководителя=Рук.КодСотрудника,
		@КодЛицаРуководителя=Рук.КодЛица,
		@Руководитель=Рук.ФИО,
		@ДолжностьРуководителя=РукДолжн.Должность
FROM		 Сотрудники Я
		INNER JOIN vwДолжности МояДолжн ON Я.КодСотрудника = МояДолжн.КодСотрудника
		INNER JOIN vwДолжности РукДолжн ON МояДолжн.Parent = РукДолжн.КодДолжности
		LEFT OUTER JOIN Сотрудники Рук ON РукДолжн.КодСотрудника = Рук.КодСотрудника
WHERE	 (Я.КодСотрудника = " + EmployeeFromField.ValueString + @") AND (МояДолжн.Совместитель = 0)

IF @@ROWCOUNT>0 AND @КодСотрудникаРуководителя IS NULL
	SELECT TOP 1  @КодЛицаРуководителя=S.КодЛица, @КодСотрудникаРуководителя=S.КодСотрудника,
				  @Руководитель=S.ФИО, @ДолжностьРуководителя=Должность
	FROM vwДолжности D
				INNER JOIN Сотрудники S ON D.КодСотрудника=S.КодСотрудника
	WHERE L<@LСотрудника AND R>@RСотрудника AND D.КодСотрудника IS NOT NULL
	ORDER BY L DESC

SELECT @Сотрудник Я, @ДолжностьСотрудника МояДолжность, @КодСотрудникаРуководителя КодСотрудникаРуководителя,
		 @КодЛицаРуководителя КодЛица, @Руководитель Руководитель, @ДолжностьРуководителя ДолжностьРуководителя
";
            var da = new SqlDataAdapter(sql, Config.DS_user);
            da.Fill(dt);

            return dt;
        }

        #region Поля документа

        /// <summary>
        ///     Лицо сотрудника
        /// </summary>
        public DocField PersonFromField { get; private set; }

        /// <summary>
        ///     От сотрудника
        /// </summary>
        public DocField EmployeeFromField { get; private set; }

        /// <summary>
        ///     От организации сотрудника
        /// </summary>
        public DocField CompanyFromField { get; private set; }

        /// <summary>
        ///     Руководителю организации
        /// </summary>
        public DocField EmployeeToField { get; private set; }

        /// <summary>
        ///     Тип отпуска
        /// </summary>
        public DocField VacationTypeField { get; private set; }

        /// <summary>
        ///     Дата начала отпуска
        /// </summary>
        public DocField DateFromField { get; private set; }

        /// <summary>
        ///     Продолжительность отпуска
        /// </summary>
        public DocField DaysField { get; private set; }

        /// <summary>
        ///     Дата конца отпуска
        /// </summary>
        public DocField DateToField { get; private set; }

        /// <summary>
        ///     Замещающий сотрудник
        /// </summary>
        public DocField SubField { get; private set; }

        #endregion

        #region GenerateText

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public string GetText()
        {
            var sb = new StringBuilder();


            sb.Append(
                "<table cellpadding='0' cellspacing='0' style=\"PADDING-TOP:1.5cm;MARGIN-LEFT: 15mm; FONT-SIZE: 10pt; WIDTH: 17cm; FONT-FAMILY: 'Times New Roman'; BORDER-COLLAPSE: collapse;\">");

            sb.Append("<tr>");
            sb.Append("<td>&nbsp;");
            sb.Append("</td>");
            MainHeader_TD(sb);
            sb.Append("</tr>");
            sb.Append("<tr>");
            Header_TD(sb);
            sb.Append("</tr>");
            sb.Append("<tr>");
            Body_TD(sb);
            sb.Append("</tr>");

            sb.Append("<tr>");
            Footer_TD(sb);
            sb.Append("</tr>");

            sb.Append("<tr>");
            MainFooter_TD(sb);
            sb.Append("</tr>");


            sb.Append("<tr>");
            BarCode_TD(sb);
            sb.Append("</tr>");


            sb.Append("</table>");
            return sb.ToString();
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public string GetText_Full()
        {
            var sb = new StringBuilder();


            sb.Append(
                "<table cellpadding='0' cellspacing='0' style=\"PADDING-TOP:1.5cm;MARGIN-LEFT: 15mm; WIDTH: 17cm; FONT-FAMILY: 'Times New Roman'; BORDER-COLLAPSE: collapse;\">");

            sb.Append("<tr>");
            sb.Append("<td>&nbsp;");
            sb.Append("</td>");
            MainHeader_TD(sb);
            sb.Append("</tr>");
            sb.Append("<tr>");
            Header_TD(sb);
            sb.Append("</tr>");
            sb.Append("<tr>");
            BodyFull_TD(sb);
            sb.Append("</tr>");
            sb.Append("<tr>");
            FooterFull_TD(sb);
            sb.Append("</tr>");
            sb.Append("<tr>");
            MainFooterFull_TD(sb);
            sb.Append("</tr>");
            sb.Append("<tr>");
            BarCode_TD(sb);
            sb.Append("</tr>");


            sb.Append("</table>");
            return sb.ToString();
        }


        private void MainHeader_TD(StringBuilder sb)
        {
            sb.Append("<td style='WIDTH:40%' valign='top'>");
            sb.Append("<table width='100%' cellpadding='0' cellspacing='0'>");
            sb.Append("<tr>");
            sb.Append("<td style='FONT-SIZE:14pt;'>");
            if (EmployeeToField.ValueString.Length > 0 && !ObjectEmployeeTo.Unavailable
                                                  && CompanyFromField.ValueString.Length > 0 &&
                                                  !ObjectCompanyFrom.Unavailable && Date != DateTime.MinValue)
                sb.Append(ObjectEmployeeTo.PersonNP_GetPostDatelPadegHead(CompanyFromField.ValueString,
                    Date.ToString("yyyyMMdd")));
            sb.Append("&nbsp;");

            if (CompanyFromField.ValueString.Length > 0 && !ObjectCompanyFrom.Unavailable)
            {
                var crd = ObjectCompanyFrom.GetCard(Date == DateTime.MinValue ? DateTime.Today : Date);
                if (crd != null) sb.Append(crd.NameRus.Length > 0 ? crd.NameRus : crd.NameLat);
            }

            sb.Append("</td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<td style='FONT-SIZE:14pt;'>");
            if (EmployeeToField.ValueString.Length > 0 && !ObjectEmployeeTo.Unavailable)
                sb.Append(ObjectEmployeeTo.PersonNP_GetFIODadelPareg(Date == DateTime.MinValue
                    ? DateTime.Today
                    : Date));
            sb.Append("</td>");
            sb.Append("</tr>");
            sb.Append("</table>");
            sb.Append("<table width='100%'>");
            sb.Append("<tr>");
            sb.Append("<td style='FONT-SIZE:14pt;'>");
            sb.Append("от");
            sb.Append("</td>");
            sb.Append("<td style='WIDTH:100%;BORDER-BOTTOM:black 1px solid'>&nbsp;</td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<td align=center colspan=2 style='FONT-SIZE: 9pt'>");
            sb.Append("(должность)");
            sb.Append("</td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<td colspan=2 style='WIDTH:100%;BORDER-BOTTOM:black 1px solid'>&nbsp;</td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<td colspan=2 style='WIDTH:100%;BORDER-BOTTOM:black 1px solid'>&nbsp;</td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<td align=center colspan=2 style='FONT-SIZE: 9pt'>");
            sb.Append("(ФИО полностью)");
            sb.Append("</td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<td colspan=2 style='WIDTH:100%;BORDER-BOTTOM:black 1px solid'>&nbsp;</td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<td colspan=2 style='WIDTH:100%;BORDER-BOTTOM:black 1px solid'>&nbsp;</td>");
            sb.Append("</tr>");
            sb.Append("</table>");


            sb.Append("</td>");
        }


        private void Header_TD(StringBuilder sb)
        {
            sb.Append("<td colspan=2 align='center' style='PADDING-TOP: 70px; FONT-SIZE:14pt;'>");
            sb.Append("<b>ЗАЯВЛЕНИЕ</b>");
            sb.Append("</td>");
        }

        private void Body_TD(StringBuilder sb)
        {
            sb.Append("<td colspan=2 style='PADDING-TOP:10px'>");
            sb.Append("<table width='100%'>");
            sb.Append("<tr>");
            sb.Append("<td nowrap style='FONT-SIZE:14pt;'>");
            sb.Append("&nbsp;&nbsp;Прошу предоставить мне");
            sb.Append("</td>");
            sb.Append("<td colspan=2 style='WIDTH:100%;BORDER-BOTTOM:black 1px solid'>&nbsp;</td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<td align=center style='FONT-SIZE: 9pt'>");
            sb.Append("&nbsp;");
            sb.Append("</td>");
            sb.Append("<td align=center colspan=2 style='FONT-SIZE:9pt;WIDTH:100%'>");
            sb.Append("(Вид отпуска)");
            sb.Append("</td>");
            sb.Append("</tr>");
            sb.Append("</table>");
            sb.Append("<table width='100%'>");
            sb.Append("<tr>");
            sb.Append("<td style='WIDTH:100%;BORDER-BOTTOM:black 1px solid'>&nbsp;</td>");
            sb.Append("<td style='FONT-SIZE:14pt;'>");
            sb.Append("продолжительностью");
            sb.Append("</td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<td align=center style='FONT-SIZE:9pt;WIDTH:100%'>");
            sb.Append("(Вид отпуска)");
            sb.Append("</td>");
            sb.Append("<td style='FONT-SIZE:9pt'>");
            sb.Append("&nbsp;");
            sb.Append("</td>");
            sb.Append("</tr>");
            sb.Append("</table>");
            sb.Append("<table>");
            sb.Append("<tr>");
            sb.Append("<td style='WIDTH:50px;BORDER-BOTTOM:black 1px solid'>&nbsp;</td>");
            sb.Append("<td nowrap style='FONT-SIZE:14pt;'>");
            sb.Append("календарных дней с ");
            sb.Append("</td>");
            sb.Append("<td>");
            sb.Append("&lt;&lt;");
            sb.Append("</td>");
            sb.Append("<td style='WIDTH:50px;BORDER-BOTTOM:black 1px solid'>&nbsp;</td>");
            sb.Append("<td width='20'>");
            sb.Append("&gt;&gt;");
            sb.Append("</td>");
            sb.Append("<td style='WIDTH:150px;BORDER-BOTTOM:black 1px solid'>&nbsp;</td>");
            sb.Append("<td width='20'>");
            sb.Append("20");
            sb.Append("</td>");
            sb.Append("<td style='WIDTH:20px;BORDER-BOTTOM:black 1px solid'>&nbsp;</td>");
            sb.Append("<td>");
            sb.Append("г.");
            sb.Append("</td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<td style='FONT-SIZE:7pt' colspan=5>");
            sb.Append("&nbsp;");
            sb.Append("</td>");
            sb.Append("<td align=center style='FONT-SIZE:9pt'>");
            sb.Append("(Дата начала отпуска)");
            sb.Append("</td>");
            sb.Append("<td style='FONT-SIZE:9pt' colspan=3>");
            sb.Append("&nbsp;");
            sb.Append("</td>");
            sb.Append("</tr>");
            sb.Append("</table>");

            sb.Append("</td>");
        }

        private void BodyFull_TD(StringBuilder sb)
        {
            sb.Append("<td colspan=2 style='PADDING-TOP:10px'>");
            sb.Append("<table width='100%'>");
            sb.Append("<tr>");
            sb.Append("<td nowrap style='FONT-SIZE:14pt;'>");
            sb.Append("&nbsp;&nbsp;Прошу предоставить мне");
            sb.Append("</td>");
            sb.Append("<td align=center colspan=2 style='WIDTH:100%;BORDER-BOTTOM:black 1px solid'>");
            if (VacationTypeField.ValueString.Length > 0 && !ObjectVacationType.Unavailable)
                sb.Append(ObjectVacationType.Name);
            else sb.Append("&nbsp;");
            sb.Append("</td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<td align=center style='FONT-SIZE: 9pt'>");
            sb.Append("&nbsp;");
            sb.Append("</td>");
            sb.Append("<td align=center colspan=2 style='FONT-SIZE:9pt;WIDTH:100%'>");
            sb.Append("(Вид отпуска)");
            sb.Append("</td>");
            sb.Append("</tr>");
            sb.Append("</table>");
            sb.Append("<table>");
            sb.Append("<tr>");
            sb.Append("<td style='FONT-SIZE:14pt;'>");
            sb.Append("продолжительностью");
            sb.Append("</td>");
            sb.Append("<td align='center' style='WIDTH:50px;BORDER-BOTTOM:black 1px solid'>");
            sb.Append(DaysField.ValueString);

            sb.Append("</td>");
            sb.Append("<td nowrap style='FONT-SIZE:14pt;'>");
            sb.Append(" календарных дней с ");
            sb.Append("</td>");
            sb.Append("<td>");
            sb.Append("&lt;&lt;");
            sb.Append("</td>");
            sb.Append("<td  align='center' style='WIDTH:50px;BORDER-BOTTOM:black 1px solid'>");
            sb.Append(((DateTime) DateFromField.Value).ToString("dd"));
            sb.Append("</td>");
            sb.Append("<td width='20'>");
            sb.Append("&gt;&gt;");
            sb.Append("</td>");
            sb.Append("<td  align='center' style='WIDTH:150px;BORDER-BOTTOM:black 1px solid'>");
            sb.Append(Dictionaries.MonthRP[((DateTime) DateFromField.Value).Month - 1]);
            sb.Append("</td>");
            sb.Append("<td  align='center' width='20'>");
            sb.Append(((DateTime) DateFromField.Value).Year);
            sb.Append("</td>");
            sb.Append("<td>");
            sb.Append("г.");
            sb.Append("</td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<td style='FONT-SIZE:9pt' colspan=6>");
            sb.Append("&nbsp;");
            sb.Append("</td>");
            sb.Append("<td align=center style='FONT-SIZE:9pt'>");
            sb.Append("(Дата начала отпуска)");
            sb.Append("</td>");
            sb.Append("<td style='FONT-SIZE:9pt' colspan=2>");
            sb.Append("&nbsp;");
            sb.Append("</td>");
            sb.Append("</tr>");
            sb.Append("</table>");
            sb.Append("</td>");
        }

        private void Footer_TD(StringBuilder sb)
        {
            sb.Append("<td colspan=2>");
            sb.Append("<table>");
            sb.Append("<tr>");
            sb.Append("<td width='20' style='PADDING-TOP:60px;' valign='bottom'>");
            sb.Append("&lt;&lt;");
            sb.Append("</td>");
            sb.Append("<td style='WIDTH:50px;BORDER-BOTTOM:black 1px solid'>&nbsp;</td>");
            sb.Append("<td width='20' valign='bottom'>");
            sb.Append("&gt;&gt;");
            sb.Append("</td>");
            sb.Append("<td style='WIDTH:150px;BORDER-BOTTOM:black 1px solid'>&nbsp;</td>");
            sb.Append("<td width='20' valign='bottom'>");
            sb.Append("20");
            sb.Append("</td>");
            sb.Append("<td style='WIDTH:20px;BORDER-BOTTOM:black 1px solid'>&nbsp;</td>");
            sb.Append("<td valign='bottom'>");
            sb.Append("г.");
            sb.Append("</td>");
            sb.Append("<td width='50'>");
            sb.Append("&nbsp;");
            sb.Append("</td>");
            sb.Append("<td style='WIDTH:100px;BORDER-BOTTOM:black 1px solid'>&nbsp;</td>");
            sb.Append("<td width='50'>");
            sb.Append("&nbsp;");
            sb.Append("</td>");
            sb.Append("<td style='WIDTH:150px;BORDER-BOTTOM:black 1px solid'>&nbsp;</td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<td style='FONT-SIZE:9pt' colspan=3>");
            sb.Append("&nbsp;");
            sb.Append("</td>");
            sb.Append("<td align=center style='FONT-SIZE:9pt'>");
            sb.Append("(Дата написания)");
            sb.Append("</td>");
            sb.Append("<td style='FONT-SIZE:9pt' colspan=4>");
            sb.Append("&nbsp;");
            sb.Append("</td>");
            sb.Append("<td align=center style='FONT-SIZE:9pt'>");
            sb.Append("(Подпись)");
            sb.Append("</td>");
            sb.Append("<td style='FONT-SIZE:9pt'>");
            sb.Append("&nbsp;");
            sb.Append("</td>");
            sb.Append("<td align=center style='FONT-SIZE:9pt'>");
            sb.Append("(ФИО)");
            sb.Append("</td>");
            sb.Append("</tr>");
            sb.Append("</table>");
            sb.Append("</td>");
        }

        private void FooterFull_TD(StringBuilder sb)
        {
            sb.Append("<td colspan=2>");
            sb.Append("<table>");
            sb.Append("<tr>");
            sb.Append("<td width='20' style='PADDING-TOP:60px;' valign='bottom'>");
            sb.Append("&lt;&lt;");
            sb.Append("</td>");
            sb.Append("<td valign='bottom'align=center style='WIDTH:50px;BORDER-BOTTOM:black 1px solid'>");
            sb.Append(Date.ToString("dd"));
            sb.Append("</td>");
            sb.Append("<td width='20' valign='bottom'>");
            sb.Append("&gt;&gt;");
            sb.Append("</td>");
            sb.Append("<td valign='bottom' align=center style='WIDTH:150px;BORDER-BOTTOM:black 1px solid'>");
            sb.Append(Dictionaries.MonthRP[Date.Month - 1]);
            sb.Append("</td>");
            sb.Append("<td width='20' align=center valign='bottom'>");
            sb.Append(Date.Year);
            sb.Append("</td>");
            sb.Append("<td valign='bottom'>");
            sb.Append("г.");
            sb.Append("</td>");
            sb.Append("<td width='50'>");
            sb.Append("&nbsp;");
            sb.Append("</td>");
            sb.Append("<td style='WIDTH:100px;BORDER-BOTTOM:black 1px solid'>&nbsp;</td>");
            sb.Append("<td width='50'>");
            sb.Append("&nbsp;");
            sb.Append("</td>");
            sb.Append("<td align='center' valign='bottom' style='WIDTH:150px;BORDER-BOTTOM:black 1px solid'>");
            if (EmployeeFromField.ValueString.Length > 0 && !ObjectEmployeeFrom.Unavailable)
                sb.Append(ObjectEmployeeFrom.FIO);
            else sb.Append("&nbsp;");


            sb.Append("</td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<td style='FONT-SIZE:9pt' colspan=3>");
            sb.Append("&nbsp;");
            sb.Append("</td>");
            sb.Append("<td align=center style='FONT-SIZE:9pt'>");
            sb.Append("(Дата написания)");
            sb.Append("</td>");
            sb.Append("<td style='FONT-SIZE:7pt' colspan=3>");
            sb.Append("&nbsp;");
            sb.Append("</td>");
            sb.Append("<td align=center style='FONT-SIZE:9pt'>");
            sb.Append("(Подпись)");
            sb.Append("</td>");
            sb.Append("<td style='FONT-SIZE:9pt'>");
            sb.Append("&nbsp;");
            sb.Append("</td>");
            sb.Append("<td align=center style='FONT-SIZE:9pt'>");
            sb.Append("(ФИО)");
            sb.Append("</td>");
            sb.Append("</tr>");
            sb.Append("</table>");
            sb.Append("</td>");
        }


        private void MainFooter_TD(StringBuilder sb)
        {
            sb.Append("<td colspan=2>");
            sb.Append("<table width='100%'>");
            sb.Append("<tr>");
            sb.Append("<td style='PADDING-TOP:40px'>");
            sb.Append("<table width='100%'>");
            sb.Append("<tr>");
            sb.Append("<td style='FONT-SIZE:14pt;'>СОГЛАСОВАНО:</td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<td style='BORDER-BOTTOM:black 1px solid'>&nbsp;</td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<td align=center style='FONT-SIZE:7pt'>");
            sb.Append("(Наименование должности руководителя отдела, где работает сотрудник)");
            sb.Append("</td>");
            sb.Append("</tr>");
            sb.Append("</table>");
            sb.Append("<table width='100%'>");
            sb.Append("<tr>");
            sb.Append("<td style='PADDING-BOTTOM:25px;WIDTH:40%;BORDER-BOTTOM:black 1px solid'>&nbsp;</td>");
            sb.Append("<td style='PADDING-BOTTOM:25px;WIDTH:10%'>&nbsp;</td>");
            sb.Append("<td style='PADDING-BOTTOM:25px;WIDTH:50%;BORDER-BOTTOM:black 1px solid'>&nbsp;</td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<td align=center style='FONT-SIZE:7pt'>");
            sb.Append("(Подпись)");
            sb.Append("</td>");
            sb.Append("<td align=center style='FONT-SIZE:7pt'>");
            sb.Append("&nbsp;");
            sb.Append("</td>");
            sb.Append("<td align=center style='FONT-SIZE:7pt'>");
            sb.Append("(ФИО)");
            sb.Append("</td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<td colspan=3>");
            sb.Append("_____._____.20__г.");
            sb.Append("</td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<td align=center style='FONT-SIZE:7pt'>");
            sb.Append("(Дата)");
            sb.Append("</td>");
            sb.Append("<td colspan=2 align=center style='FONT-SIZE:7pt'>");
            sb.Append("&nbsp;");
            sb.Append("</td>");
            sb.Append("</tr>");
            sb.Append("</table>");
            sb.Append("</td>");
            sb.Append("<td width='50%'>&nbsp;</td>");
            sb.Append("</tr>");
            sb.Append("</table>");
            sb.Append("</td>");
        }

        private void MainFooterFull_TD(StringBuilder sb)
        {
            var dt = GetPostSupervisor();
            var _postS = "&nbsp;";
            var _fioS = "&nbsp;";
            if (dt.Rows.Count > 0 && !EmployeeToField.ValueString.Equals(dt.Rows[0]["КодЛица"].ToString()))
            {
                _postS = dt.Rows[0]["ДолжностьРуководителя"].Equals(DBNull.Value) ||
                         dt.Rows[0]["ДолжностьРуководителя"].Equals("")
                    ? _postS
                    : dt.Rows[0]["ДолжностьРуководителя"].ToString();
                _fioS = dt.Rows[0]["Руководитель"].Equals(DBNull.Value) || dt.Rows[0]["Руководитель"].Equals("")
                    ? _fioS
                    : dt.Rows[0]["Руководитель"].ToString();
            }

            sb.Append("<td colspan=2>");
            sb.Append("<table width='100%'>");
            sb.Append("<tr>");
            sb.Append("<td style='PADDING-TOP:40px'>");
            sb.Append("<table width='100%'>");
            sb.Append("<tr>");
            sb.Append("<td style='FONT-SIZE:12pt;'>СОГЛАСОВАНО:</td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<td align='center' style='BORDER-BOTTOM:black 1px solid'>");
            sb.Append(_postS);
            sb.Append("</td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<td align=center style='FONT-SIZE:7pt'>");
            sb.Append("(Наименование должности руководителя отдела, где работает сотрудник)");
            sb.Append("</td>");
            sb.Append("</tr>");
            sb.Append("</table>");
            sb.Append("<table width='100%'>");
            sb.Append("<tr>");
            sb.Append("<td style='PADDING-BOTTOM:25px;WIDTH:40%;BORDER-BOTTOM:black 1px solid'>&nbsp;</td>");
            sb.Append("<td style='PADDING-BOTTOM:25px;WIDTH:10%'>&nbsp;</td>");
            sb.Append("<td align='PADDING-BOTTOM:25px;center' style='WIDTH:50%;BORDER-BOTTOM:black 1px solid'>");
            sb.Append(_fioS);
            sb.Append("</td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<td align=center style='FONT-SIZE:7pt'>");
            sb.Append("(Подпись)");
            sb.Append("</td>");
            sb.Append("<td align=center style='FONT-SIZE:7pt'>");
            sb.Append("&nbsp;");
            sb.Append("</td>");
            sb.Append("<td align=center style='FONT-SIZE:7pt'>");
            sb.Append("(ФИО)");
            sb.Append("</td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<td colspan=3>");
            sb.Append("_____._____.20__г.");
            sb.Append("</td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<td align=center style='FONT-SIZE:7pt'>");
            sb.Append("(Дата)");
            sb.Append("</td>");
            sb.Append("<td colspan=2 align=center style='FONT-SIZE:7pt'>");
            sb.Append("&nbsp;");
            sb.Append("</td>");
            sb.Append("</tr>");
            sb.Append("</table>");
            sb.Append("</td>");
            sb.Append("<td width='50%'>&nbsp;</td>");
            sb.Append("</tr>");
            sb.Append("</table>");
            sb.Append("</td>");
        }

        private void BarCode_TD(StringBuilder sb)
        {
            sb.Append("<td colspan=2 align='right'>");
            sb.Append("<img border=0 src='barcode.ashx?id=" + Id + "'>");
            sb.Append("<span style='WIDTH:25px'>&nbsp;</span>");
            sb.Append("</td>");
        }

        #endregion
    }
}