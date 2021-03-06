﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Kesco.Lib.BaseExtention;
using Kesco.Lib.BaseExtention.Enums.Docs;
using Kesco.Lib.DALC;
using Kesco.Lib.Entities.Corporate;
using Kesco.Lib.Entities.Resources;
using Convert = Kesco.Lib.ConvertExtention.Convert;

namespace Kesco.Lib.Entities.Documents.EF.Dogovora
{
    /// <summary>
    ///     Договор (класс также является базовым для приложения к договору)
    /// </summary>
    [Serializable]
    public class Dogovor : Document, IDocumentWithPositions
    {
        /// <summary>
        ///     Backing field для свойства ColumnAndFields
        /// </summary>
        private Dictionary<string, DocField> _fields;

        private readonly DateTime DatePayment = DateTime.MinValue;

        private readonly DateTime DateTrade = DateTime.MinValue;

        private readonly DateTime DatePredoplata = DateTime.MinValue;

        private DateTime DateUniversal = DateTime.MinValue;

        /// <summary>
        ///     Конструктор по умолчанию
        /// </summary>
        public Dogovor()
        {
        }

        /// <summary>
        ///     Конструктор с загрузкой документа
        /// </summary>
        public Dogovor(string id)
        {
            LoadDocument(id, true);
        }

        /// <summary>
        ///     Начало действия
        /// </summary>
        public DocField ValidFromField => GetFieldByColumnName("Дата2");

        /// <summary>
        ///     Окончание действия
        /// </summary>
        public DocField ValidTillField => GetFieldByColumnName("Дата3");

        /// <summary>
        ///     Валюта оплаты
        /// </summary>
        public DocField ValyutaField => GetFieldByColumnName("КодРесурса2");

        /// <summary>
        ///     Расчеты в у.е.
        /// </summary>
        public DocField UEField => GetFieldByColumnName("Flag1");

        /// <summary>
        ///     Формула
        /// </summary>
        public DocField FormulaField => GetFieldByColumnName("Text100_3");

        /// <summary>
        ///     Формула описание
        /// </summary>
        public DocField FormulaDescrField => GetFieldByColumnName("Text300_2");

        /// <summary>
        ///     Тип договора для печати в печатных документах Rus
        /// </summary>
        public DocField NameRusField => GetFieldByColumnName("Text100_1");

        /// <summary>
        ///     Тип договора для печати в печатных документах Lat
        /// </summary>
        public DocField NameEngField => GetFieldByColumnName("Text100_2");

        /// <summary>
        ///     Вид взаиморасчетов
        /// </summary>
        public DocField VidVzaimoraschetovField => GetFieldByColumnName("Int1");

        /// <summary>
        ///     Куратор
        /// </summary>
        public DocField KuratorField => GetFieldByColumnName("КодСотрудника1");

        /// <summary>
        ///     Сумма договора
        /// </summary>
        public DocField SummaField => GetFieldByColumnName("Money1");

        /// <summary>
        ///     Поле КодЛица1
        /// </summary>
        public DocField Person1Field => GetFieldByColumnName("КодЛица1");

        /// <summary>
        ///     Поле КодЛица2
        /// </summary>
        public DocField Person2Field => GetFieldByColumnName("КодЛица2");

        /// <summary>
        ///     Поле КодЛица3
        /// </summary>
        public DocField Person3Field => GetFieldByColumnName("КодЛица3");

        /// <summary>
        ///     Поле КодЛица4
        /// </summary>
        public DocField Person4Field => GetFieldByColumnName("КодЛица4");


        /// <summary>
        ///     Поле КодСклада1
        /// </summary>
        public DocField Sklad1Field => GetFieldByColumnName("КодСклада1");

        /// <summary>
        ///     Поле КодСклада2
        /// </summary>
        public DocField Sklad2Field => GetFieldByColumnName("КодСклада2");

        /// <summary>
        ///     Поле КодСклада3
        /// </summary>
        public DocField Sklad3Field => GetFieldByColumnName("КодСклада3");

        /// <summary>
        ///     Поле КодСклада4
        /// </summary>
        public DocField Sklad4Field => GetFieldByColumnName("КодСклада4");

        /// <summary>
        ///     Поле Text50_1
        /// </summary>
        public DocField EmplPerson1Field => GetFieldByColumnName("Text50_1");

        /// <summary>
        ///     Поле Text50_2
        /// </summary>
        public DocField PostEmplPerson1Field => GetFieldByColumnName("Text50_2");

        /// <summary>
        ///     Поле Text50_3
        /// </summary>
        public DocField EmplPerson2Field => GetFieldByColumnName("Text50_3");

        /// <summary>
        ///     Поле Text50_4
        /// </summary>
        public DocField PostEmplPerson2Field => GetFieldByColumnName("Text50_4");

        /// <summary>
        ///     Поле КодСтатьиБюджета
        /// </summary>
        public DocField BudgetLineField => GetFieldByColumnName("КодСтатьиБюджета");

        /// <summary>
        ///     Поле Int6
        /// </summary>
        public DocField PayerBankCommissionsField => GetFieldByColumnName("Int6");

        /// <summary>
        ///     Номер контракта пока не известен
        /// </summary>
        public DocField NumberOptionsField => GetFieldByColumnName("Int7");

        /// <summary>
        ///     Куратор
        /// </summary>
        public string _Kurator => KuratorField == null ? "" : KuratorField.ValueString;

        /// <summary>
        ///     Куратор
        /// </summary>
        public Employee Kurator => new Employee(_Kurator);

        /// <summary>
        /// </summary>
        public string _ValidFrom => ValidFromField.IsValueEmpty ? "" : ValidFromField.ValueString;

        /// <summary>
        /// </summary>
        public string _ValidTill => ValidTillField.IsValueEmpty ? "" : ValidTillField.ValueString;

        /// <summary>
        ///     Поля документа с ключем по названию колонки
        /// </summary>
        public Dictionary<string, DocField> ColumnAndFields
        {
            get
            {
                if (_fields == null)
                {
                    _fields = new Dictionary<string, DocField>();
                    if (Fields != null)
                        foreach (var v in Fields.Values)
                            if (!_fields.ContainsKey(v.DataColomnName))
                                _fields.Add(v.DataColomnName, v);
                }

                return _fields;
            }
        }

        /// <summary>
        ///     Расчеты в у.е.
        /// </summary>
        public bool UE
        {
            get
            {
                if (UEField.DocFieldId == 0) return false;
                if (UEField.Value == null) return false;
                var ue = (byte) UEField.Value;
                return UEField != null && ue == 1;
            }
        }


        /// <summary>
        ///     Получить название на русском
        /// </summary>
        public string NameRusFull
        {
            get
            {
                var b = new StringBuilder();
                b.Append(NameRusField.ValueString);
                if (string.IsNullOrEmpty(Number)) b.Append(string.Format(" № {0}", Number));
                if (Date > DateTime.MinValue) b.Append(" от " + Date.ToString("dd.MM.yyyy"));

                return b.ToString();
            }
        }

        /// <summary>
        ///     Получить название на английском
        /// </summary>
        public string NameEngFull
        {
            get
            {
                var b = new StringBuilder();
                b.Append(NameEngField.ValueString);
                if (string.IsNullOrEmpty(Number)) b.Append(string.Format(" № {0}", Number));
                if (Date > DateTime.MinValue) b.Append(" dd " + Date.ToString("dd.MM.yyyy"));

                return b.ToString();
            }
        }

        /// <summary>
        ///     Валюта
        /// </summary>
        public string Valyuta => ValyutaField == null ? "" : ValyutaField.ValueInt.ToString();

        /// <summary>
        /// </summary>
        public bool ValidTillUnlimited => _ValidTill.Equals("20500101");

        /// <summary>
        /// </summary>
        public DateTime ValidFrom => ValidFromField.DateTimeValue ?? DateTime.MinValue;

        /// <summary>
        /// </summary>
        public DateTime ValidTill => ValidTillField.DateTimeValue ?? DateTime.MinValue;

        /// <summary>
        ///     Позиции документа
        /// </summary>
        public List<DogovorPosition> DogovorPositionList { get; set; }

        /// <summary>
        ///     Сохранение позиций документа
        /// </summary>
        public void SaveDocumentPositions(bool reloadPostions, List<DBCommand> cmds = null)
        {
            var documentPosition = DocumentPosition<DogovorPosition>.LoadByDocId(int.Parse(Id));

            documentPosition.ForEach(delegate(DogovorPosition p0)
            {
                var p = DogovorPositionList.FirstOrDefault(x => x.Id == p0.PositionId.ToString());
                if (p == null)
                    p0.Delete(false);
            });

            DogovorPositionList.ForEach(delegate(DogovorPosition p)
            {
                if (string.IsNullOrEmpty(p.PositionId.ToString()))
                {
                    p.DocumentId = int.Parse(Id);
                    p.Save(reloadPostions, cmds);
                    return;
                }

                var p0 =
                    documentPosition.FirstOrDefault(
                        x => x.Id == p.Id && x.PositionId != p.PositionId);
                if (p0 != null) p.Save(reloadPostions, cmds);
            });
        }

        /// <summary>
        ///     Загрузка позиций документа
        /// </summary>
        public void LoadDocumentPositions()
        {
            LoadDogovorPosition();
        }

        /// <summary>
        ///     Получает поле документа по названию колонки(таблица ПоляДокументов)
        /// </summary>
        public DocField GetFieldByColumnName(string columnName)
        {
            if (ColumnAndFields != null)
                if (ColumnAndFields.ContainsKey(columnName))
                    return ColumnAndFields[columnName];

            return new DocField(this);
        }

        /// <summary>
        /// </summary>
        /// <param name="Date"></param>
        /// <returns></returns>
        public decimal GetCoefUe2Valuta(DateTime Date)
        {
            if (!UE) return 1M;
            DateUniversal = Date;
            return _GetCoefUe2Valuta();
        }

        /// <summary>
        /// </summary>
        /// <remarks>Скопировано из v2</remarks>
        private decimal _GetCoefUe2Valuta()
        {
            var formula = FormulaField.ValueString;

            try
            {
                return decimal.Parse(formula.Replace('.', ','));
            }
            catch
            {
            }

            var f = formula.Replace(" ", "");
            f = f.Replace("ДатаОплаты", "ДатаОплаты()");
            f = f.Replace("ДатаРеализации", "ДатаРеализации()");
            f = f.Replace("ДатаСчета", "ДатаСчета()");
            f = ReplacePercent(f, "Percent");
            f = ReplaceOperator(f, "Multiply", '*');
            f = ReplaceOperator(f, "Devide", '/');
            f = ReplaceOperator(f, "Minus", '-');
            f = ReplaceOperator(f, "Plus", '+');


            return Convert.Round((decimal) Exec(f), 8);
        }

        /// <summary>
        /// </summary>
        /// <remarks>Скопировано из v2</remarks>
        private object Exec(string f)
        {
            object rez;
            var ind = f.IndexOf('(');

            var fName = f.Substring(0, ind);

            if (f[f.Length - 1] != ')')
                throw new Exception("Нет закрывающей скобочки для функции " + fName);

            var mf = GetType().GetMethod(fName,
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);

            if (mf == null)
                mf = GetType().BaseType.GetMethod(fName, BindingFlags.Instance | BindingFlags.NonPublic);

            if (mf != null)
            {
                var args = new ArrayList(); //10mf.GetParameters().Length
                var strArg = f.Substring(ind + 1, f.Length - ind - 2); //уберем первую и последнюю скобочки

                var p = "";
                var index = 0;

                for (var j = 0; j < strArg.Length; j++)
                {
                    var x = strArg[j];
                    p += x;
                    if (x == '(') index++;
                    if (x == ')') index--;

                    if (index == 0 && (strArg[j] == ',' || j == strArg.Length - 1)
                    ) // (запятой разделяем аргументы функции) или конец строки аргументов
                    {
                        if (strArg[j] == ',')
                            p = p.Remove(p.Length - 1, 1);

                        if (p.IndexOf('(') != -1) //если аргумент содержит скобочку, значит это функция
                        {
                            args.Add(Exec(p));
                        }
                        else
                        {
                            if (Regex.IsMatch(p, @"(^\d+(\,|\.)\d+$)|(^\d+(\,|\.)$)|(^(\,|\.)\d+$)|(^\d+$)"))
                                args.Add(decimal.Parse(p.Replace('.', ',')));
                            else
                                args.Add(p);
                        }

                        p = "";
                    }
                }

                try
                {
                    if (mf.Name.Equals("MIN"))
                        rez = MIN(args.ToArray());
                    else if (mf.Name.Equals("MAX"))
                        rez = MAX(args.ToArray());
                    else if (mf.Name.Equals("AVG"))
                        rez = AVG((decimal[]) args.ToArray(typeof(decimal)));
                    else
                        rez = mf.Invoke(this, args.ToArray());
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.InnerException.Message);
                }
            }
            else
            {
                throw new Exception("Неизвестная функция " + fName);
            }

            return rez;
        }

        /// <summary>
        /// </summary>
        public bool IsValidAt(DateTime date)
        {
            bool res;
            switch (GetValidAtStatus(date))
            {
                case ValidAtStatuses.Undecidable:
                    throw new Exception("Некорректно указан срок действия договора (или приложения) с кодом " + Id);
                case ValidAtStatuses.Valid:
                    res = true;
                    break;
                default:
                    res = false;
                    break;
            }

            return res;
        }

        /// <summary>
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public ValidAtStatuses GetValidAtStatus(DateTime date)
        {
            if (DocType.ChildOf(DocTypeEnum.Договор) || DocType.ChildOf(DocTypeEnum.Приложение))
            {
                if (DataUnavailable) return ValidAtStatuses.NotValid;
                if (_ValidFrom.Length == 0 || _ValidTill.Length == 0) return ValidAtStatuses.Undecidable;
                if (date < ValidFrom) return ValidAtStatuses.NotValidYet;
                if (!ValidTillUnlimited && date >= ValidTill) return ValidAtStatuses.NotValidAlready;
            }

            return ValidAtStatuses.Valid;
        }

        /// <summary>
        /// </summary>
        /// <remarks>Скопировано из v2</remarks>
        private object MIN(params object[] val)
        {
            var ar = new ArrayList(val);
            ar.Sort();
            return ar[0];
        }

        /// <summary>
        /// </summary>
        /// <remarks>Скопировано из v2</remarks>
        private object MAX(params object[] val)
        {
            var ar = new ArrayList(val);
            ar.Sort();
            return ar[ar.Count - 1];
        }

        /// <summary>
        /// </summary>
        /// <remarks>Скопировано из v2</remarks>
        private object AVG(params decimal[] val)
        {
            if (val.Length == 0) return 0m;
            var rez = val.Sum();

            return rez / val.Length;
        }

        /// <summary>
        /// </summary>
        /// <remarks>Скопировано из v2</remarks>
        private string ReplacePercent(string f, string opName)
        {
            var ind = f.IndexOf('%');
            var endRight = ind + 1;
            if (ind != -1)
            {
                //найдем левый операнд
                var index = 0; //индекс скобочки
                var startLeft = 0;
                for (var j = ind - 1; j > 0; j--)
                {
                    if (f[j] == '(') index--;
                    if (f[j] == ')') index++;
                    if (index == 0)
                    {
                        if (j == 0) //если дальше идти не куда
                        {
                            startLeft = 0;
                        }
                        else
                        {
                            if (char.IsLetterOrDigit(f[j - 1]) || f[j - 1] == '.')
                                while (j > 0 && (char.IsLetterOrDigit(f[j - 1]) || f[j - 1] == '.'))
                                    j--;

                            startLeft = j;
                            break;
                        }
                    }
                }

                var Percent = f.Substring(startLeft, ind - startLeft);
                //---------------------------

                //найдем левый операнд до + или -
                if (!(f[startLeft - 1] == '+' || f[startLeft - 1] == '-'))
                    throw new Exception("Ошибка в записи процента");

                var PlusMinus = f[startLeft - 1] == '+' ? '1' : '0';
                ind = startLeft - 1;
                startLeft = 0;

                for (var j = ind - 1; j > 0; j--)
                {
                    if (f[j] == '(') index--;
                    if (f[j] == ')') index++;
                    if (index == 0)
                    {
                        if (j == 0) //если дальше идти не куда
                        {
                            startLeft = 0;
                        }
                        else
                        {
                            if (char.IsLetterOrDigit(f[j - 1]) || f[j - 1] == '.')
                                while (j > 0 && (char.IsLetterOrDigit(f[j - 1]) || f[j - 1] == '.'))
                                    j--;

                            startLeft = j;
                            break;
                        }
                    }
                }

                var LeftOperand = f.Substring(startLeft, ind - startLeft);
                //--------------------


                var Func = opName + "(" + DeleteBrackets(LeftOperand) + "," + DeleteBrackets(Percent) + "," +
                           PlusMinus + ")";

                var rez = (startLeft > 0 ? f.Substring(0, startLeft) : "") + Func +
                          (endRight == f.Length ? "" : f.Substring(endRight, f.Length - endRight));

                if (rez.IndexOf('%') > 0)
                    rez = ReplacePercent(rez, opName);
                return rez;
            }

            return f;
        }

        /// <summary>
        /// </summary>
        /// <remarks>Скопировано из v2</remarks>
        private string DeleteBrackets(string x)
        {
            if (x.Length < 2) return x;
            if (x[0] == '(' && x[x.Length - 1] == ')')
            {
                x = x.Substring(1, x.Length - 2);
                if (x.Length > 0 && x[0] == '(') x = DeleteBrackets(x);
            }

            return x;
        }

        /// <summary>
        /// </summary>
        /// <remarks>Скопировано из v2</remarks>
        private string ReplaceOperator(string f, string opName, char opSymbol)
        {
            var ind = f.IndexOf(opSymbol);
            if (ind > 0)
            {
                //найдем левый операнд
                var index = 0; //индекс скобочки
                var startLeft = 0;
                for (var j = ind - 1; j > 0; j--)
                {
                    if (f[j] == '(') index--;
                    if (f[j] == ')') index++;
                    if (index == 0)
                    {
                        if (j == 0) //если дальше идти не куда
                        {
                            startLeft = 0;
                        }
                        else
                        {
                            if (char.IsLetterOrDigit(f[j - 1]) || f[j - 1] == '.')
                                while (j > 0 && (char.IsLetterOrDigit(f[j - 1]) || f[j - 1] == '.'))
                                    j--;

                            startLeft = j;
                            break;
                        }
                    }
                }


                var LeftOperand = f.Substring(startLeft, ind - startLeft);

                //Найдем правый операнд
                index = 0;
                var endRight = ind;
                endRight++;
                if (endRight >= f.Length)
                    throw new Exception("Ошибка в формуле.");

                if (f[endRight] == '(') //если выражение в скобках
                {
                    do
                    {
                        if (f[endRight] == '(') index++;
                        if (f[endRight] == ')') index--;
                        endRight++;
                    } while (endRight < f.Length && index > 0);
                }
                else if (char.IsLetter(f[endRight])) //если функция
                {
                    while (endRight < f.Length && char.IsLetterOrDigit(f[endRight]))
                        endRight++;
                    //дошли до конца названия функции

                    var fName = f.Substring(ind + 1, endRight - ind - 1);

                    if (endRight == f.Length)
                        throw new Exception("Неизвестная строковая константа: " + fName);


                    //теперь добавим аргументы функции
                    if (f[endRight] == '(')
                        do
                        {
                            if (f[endRight] == '(') index++;
                            if (f[endRight] == ')') index--;
                            endRight++;
                        } while (endRight < f.Length && index > 0);
                }
                else if (char.IsDigit(f[endRight]) || f[endRight] == '.') //число
                {
                    while (endRight < f.Length && (char.IsDigit(f[endRight]) || f[endRight] == '.'))
                        endRight++;
                }


                var RightOperand = f.Substring(ind + 1, endRight - ind - 1);

                //Выполним замену оператора функцией
                var Func = opName + "(" + DeleteBrackets(LeftOperand) + "," + DeleteBrackets(RightOperand) + ")";

                var rez = (startLeft > 0 ? f.Substring(0, startLeft) : "") + Func +
                          (endRight == f.Length ? "" : f.Substring(endRight, f.Length - endRight));

                if (rez.IndexOf(opSymbol) > 0)
                    rez = ReplaceOperator(rez, opName, opSymbol);

                return rez;
            }

            return f;
        }

        /// <summary>
        ///     Загрузка позиций документа
        /// </summary>
        private void LoadDogovorPosition()
        {
            if (Id.IsNullEmptyOrZero())
                DogovorPositionList = new List<DogovorPosition>();
            else
                DogovorPositionList = DocumentPosition<DogovorPosition>.LoadByDocId(int.Parse(Id));
        }

        private decimal КурсЦБРФ(string Valuta, DateTime Date)
        {
            if (DateUniversal != DateTime.MinValue)
                Date = DateUniversal;

            if (Date == DateTime.MinValue)
                throw new Exception("Неврено указана дата в формуле расчета у.е.");

            Valuta = Valuta.ToUpper().Trim();

            var curId = Currency.GetCurrencyByName(Valuta);
            if (curId == 0)
                throw new Exception("Неправильно записана валюта: " + Valuta);

            return Currency.GetKursCbrf(curId, Date);
        }

        private DateTime ДатаОплаты()
        {
            if (DatePayment == DateTime.MinValue && DateUniversal == DateTime.MinValue)
                throw new Exception("Не передана дата оплаты для определения коэффициента пересчета у.е.");
            return DatePayment;
        }

        private DateTime ДатаРеализации()
        {
            if (DateTrade == DateTime.MinValue && DateUniversal == DateTime.MinValue)
                throw new Exception("Не передана дата реализации для определения коэффициента пересчета у.е.");
            return DateTrade;
        }

        private DateTime ДатаСчета()
        {
            if (DatePredoplata == DateTime.MinValue && DateUniversal == DateTime.MinValue)
                throw new Exception("Не передана дата счета для определения коэффициента пересчета у.е.");
            return DatePredoplata;
        }

        private decimal Kotirovka(string s)
        {
            return 0m;
        }
    }
}