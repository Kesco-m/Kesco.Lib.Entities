using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Kesco.Lib.Entities.Corporate.Phones
{
    /// <summary>
    /// Класс обеспечивающий работу с телефонными номера в инвентаризации
    /// </summary>
    public class Phone
    {
        /// <summary>
        /// Процедура разбора телефонного номера на части
        /// </summary>
        /// <param name="textNum">Полный телефонный номер, которвый нужно разобрать</param>
        /// <param name="p_areaName">Название территории</param>
        /// <param name="p_telCodeCountry">Телефонный код страны</param>
        /// <param name="p_telCodeInCountry">Телефонный код в стране</param>
        /// <param name="p_direction">Направление</param>
        /// <param name="p_phone">Местный номер</param>
        public static void GetPhoneNumberInfo(string textNum, ref string p_areaName, ref string p_telCodeCountry, ref string p_telCodeInCountry,
            ref string p_direction, ref string p_phone)
        {
            
            int p_dlinaCoda = 0;

            p_phone = "";
            p_direction="";
            p_telCodeCountry="";
            p_telCodeInCountry="";
            p_areaName = "";

            textNum = Regex.Replace(textNum, @"\D", "");

            if (textNum.Length == 0) return;

            if (p_telCodeCountry.Trim().Length == 0)
            {
                if (textNum.StartsWith("810"))
                    textNum = "+" + textNum.Remove(0, 3);
                if (textNum.StartsWith("00"))
                    textNum = "+" + textNum.Remove(0, 2);
                else if (textNum.StartsWith("8"))
                    textNum = "+7" + textNum.Remove(0, 1);

                if (textNum.StartsWith("+"))
                    textNum = textNum.Remove(0, 1);

                if (textNum.Length == 0) return;
            }


            Dictionary<string, object> inputParameters = new Dictionary<string, object>();
            Dictionary<string, object> outpuParameters = new Dictionary<string, object>();

            //--------------------------

            inputParameters.Add("@Телефон", textNum);

            outpuParameters.Add("@Направление",         p_direction);
            outpuParameters.Add("@ТелКодСтраны",        p_telCodeCountry);
            outpuParameters.Add("@ТелКодВСтране",       p_telCodeInCountry);
            outpuParameters.Add("@ДлинаКодаОбласти",    p_dlinaCoda);
            outpuParameters.Add("@Территория",          p_areaName);
            

            DALC.DBManager.ExecuteNonQuery(SQLQueries.SELECT_ЧастиТелефонногоНомера, CommandType.Text, Kesco.Lib.Web.Settings.Config.DS_user, inputParameters, outpuParameters);

            p_direction         = outpuParameters["@Направление"].ToString();
            p_telCodeCountry    = outpuParameters["@ТелКодСтраны"].ToString();
            p_telCodeInCountry  = outpuParameters["@ТелКодВСтране"].ToString();
            p_dlinaCoda = outpuParameters["@ДлинаКодаОбласти"] == null || outpuParameters["@ДлинаКодаОбласти"].ToString().Length==0 ? 0 : int.Parse(outpuParameters["@ДлинаКодаОбласти"].ToString());
            p_areaName          = outpuParameters["@Территория"].ToString();

            //--------------------------
            
            string phoneNum = textNum.Remove(0, string.Concat(p_telCodeCountry, p_telCodeInCountry).Length);
            if (phoneNum.Length > 0)
            {
                string Phone2 = "";
                //возмьем из исходного номера столько цифр с конца пока не получится номер Phone2
                for (int i = 0; i < textNum.Length; i++)
                {
                    p_phone = textNum[textNum.Length - 1 - i] + p_phone;

                    if (char.IsDigit(textNum[textNum.Length - 1 - i]))
                        Phone2 = textNum[textNum.Length - 1 - i] + Phone2;

                    if (phoneNum == Phone2)
                        break;
                }
            }

            int lengthCode = Math.Min(p_dlinaCoda, p_telCodeInCountry.Length);
            p_phone = p_telCodeInCountry.Substring(lengthCode, p_telCodeInCountry.Length - lengthCode) + p_phone;
            p_telCodeInCountry = p_telCodeInCountry.Substring(0, lengthCode);

        }
    }
}
