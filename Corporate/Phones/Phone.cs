using System;
using System.Text.RegularExpressions;

namespace Kesco.Lib.Entities.Corporate.Phones
{
    /// <summary>
    ///     Класс обеспечивающий работу с телефонными номера в инвентаризации
    /// </summary>
    public class Phone
    {
        /// <summary>
        ///     Разбитие телефонного номера на части
        /// </summary>
        /// <param name="area">Информация о частях телефонного номера</param>
        /// <param name="phone">Телефонный номер</param>
        public static void AdjustPhoneNumber(ref AreaPhoneInfo area, ref string phone)
        {
            var number = Regex.Replace(area.CountryPhoneCode + area.PhoneCodeInCountry + phone, @"\D", "");
            phone = string.Empty;

            //сделаем замены в номере
            if (area.CountryPhoneCode.Trim().Length == 0 && area.PhoneCodeInCountry.Trim().Length == 0)
            {
                if (number.StartsWith("810"))
                    number = "+" + number.Remove(0, 3);
                else if (number.StartsWith("00"))
                    number = "+" + number.Remove(0, 2);
                else if (number.StartsWith("8"))
                    number = "+7" + number.Remove(0, 1);

                if (number.StartsWith("+"))
                    number = number.Remove(0, 1);
                
            }

            if (number.Length == 0) return;

            if (area.CountryPhoneCode.Trim().Length == 0 && area.PhoneCodeInCountry.Trim().Length > 0) return;

            var areaInfo2 = AreaPhoneInfo.GetAreaPhoneInfo(number);

            if (areaInfo2 == null) return;

            area = areaInfo2;

            var countryPhoneCode = area.CountryPhoneCode;
            var codeLength = area.LengthPhoneCodeInCountry ?? 0;

            var phoneWithoutCountry = number;
            if (phoneWithoutCountry.StartsWith(countryPhoneCode))
            {
                phoneWithoutCountry = number.Remove(0, countryPhoneCode.Length);
                area.PhoneCodeInCountry =
                    phoneWithoutCountry.Substring(0, Math.Min(codeLength, phoneWithoutCountry.Length));
            }

            var phoneCode = string.Concat(countryPhoneCode, area.PhoneCodeInCountry);
            var phoneNum = number;
            if (number.StartsWith(phoneCode)) phoneNum = number.Remove(0, phoneCode.Length);
            if (phoneNum.Length > 0)
            {
                var phone2 = "";
                //возмьем из исходного номера столько цифр с конца пока не получится номер Phone2
                for (var i = 0; i < number.Length; i++)
                {
                    phone = number[number.Length - 1 - i] + phone;

                    if (char.IsDigit(number[number.Length - 1 - i]))
                        phone2 = number[number.Length - 1 - i] + phone2;

                    if (phoneNum == phone2)
                        break;
                }
            }

            var length = Math.Min(codeLength, area.PhoneCodeInCountry.Length);
            phone = area.PhoneCodeInCountry.Substring(length, area.PhoneCodeInCountry.Length - length) + phone;
            area.PhoneCodeInCountry = area.PhoneCodeInCountry.Substring(0, length);
        }
    }
}