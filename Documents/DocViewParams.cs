using System;
using System.Collections.Generic;
using System.Data;
using Kesco.Lib.DALC;
using Kesco.Lib.Web.Settings;

namespace Kesco.Lib.Entities.Documents
{

    /// <summary>
    /// Параметры DocView из БД. Cтатический класс конфига параметров DocView.
    /// </summary>
    /// <remarks>
    ///  Большинство полей кешируются через предикат if (!_available).
    ///  Так же возможно принудительное обновление через метод Refresh()
    /// </remarks>
    /// <example>
    /// Примеры использования и юнит тесты: Kesco.App.UnitTests.DalcTests.DocumentsTest
    /// </example>
    public static class DocViewParams
    {
        /// <summary>
        ///  Connection string для базы документы. 
        ///  Readonly - получает только один раз по необходимости за работу приложения.
        /// </summary>
        private static readonly string CN = Config.DS_document;

        /// <summary>
        ///  Доступность сущности. Загружены ли данные или нет. 
        /// </summary>
        private static bool _available;


        #region Backing field для свойств

        private static int _codePerson;
        private static string _orderGroup;
        private static string _codeDocumentLink;
        private static bool _messagesNotify;
        private static int _readMarkTime;
        private static bool _sublevelDocuments;
        private static short _archiveDateFilter;
        private static short _dateDocumentFilter;
        private static short _creationDateFilter;
        private static bool _deleteConfirm;
        private static bool _groupOperationsConfirm;
        private static bool _showNews;
        private static bool _addWorkSaving;
        private static bool _savingOpenSaved;
        private static bool _sendMessageSaving;
        private static bool _signMessageWorkDone;
        private static bool _incomingFaxesJustNotSaved;
        private static bool _sendFaxesJustNotSaved;
        private static bool _moveNextSendingMessages;
        private static bool _searchMultipleDocumentsBarcode;
        private static bool _personalSendListFirstOrder;
        private static byte _readMessageEndWork;

        #endregion

        #region Поля сущности

        /// <summary>
        ///  Поле КодЛица
        /// </summary>
        /// <value>
        /// КодЛица (int, null)
        /// </value>
        public static int CodePerson
        {
            get
            {
                if (!_available) LoadDocViewParams();
                return _codePerson;
            }
            set { _codePerson = value; }
        }

        /// <summary>
        ///  Поле ПорядокГруппировки
        /// </summary>
        /// <value>
        /// ПорядокГруппировки (varchar(10), not null)
        /// </value>
        public static string OrderGroup
        {
            get
            {
                if (!_available) LoadDocViewParams();
                return _orderGroup;
            }
            set { _orderGroup = value; }
        }

        /// <summary>
        ///  Поле КодыДокументовСвязующих
        /// </summary>
        /// <value>
        /// КодыДокументовСвязующих (varchar(1000), not null)
        /// </value>
        public static string CodeDocumentLink
        {
            get
            {
                if (!_available) LoadDocViewParams();
                return _codeDocumentLink;
            }
            set { _codeDocumentLink = value; }
        }

        /// <summary>
        ///  Поле УведомлениеСообщения
        /// </summary>
        /// <value>
        /// УведомлениеСообщения (bit, not null)
        /// </value>
        public static bool MessagesNotify
        {
            get
            {
                if (!_available) LoadDocViewParams();
                return _messagesNotify;
            }
            set { _messagesNotify = value; }
        }

        /// <summary>
        ///  Поле ВремяОтметкиПрочтения
        /// </summary>
        /// <value>
        /// ВремяОтметкиПрочтения (int, not null)
        /// </value>
        public static int ReadMarkTime
        {
            get
            {
                if (!_available) LoadDocViewParams();
                return _readMarkTime;
            }
            set { _readMarkTime = value; }
        }

        /// <summary>
        ///  Поле ДокументыПодуровней
        /// </summary>
        /// <value>
        /// ДокументыПодуровней (bit, not null)
        /// </value>
        public static bool SublevelDocuments
        {
            get
            {
                if (!_available) LoadDocViewParams();
                return _sublevelDocuments;
            }
            set { _sublevelDocuments = value; }
        }

        /// <summary>
        ///  Поле ФильтрДатыАрхивирования
        /// </summary>
        /// <value>
        /// ФильтрДатыАрхивирования (smallint, not null)
        /// </value>
        public static Int16 ArchiveDateFilter
        {
            get
            {
                if (!_available) LoadDocViewParams();
                return _archiveDateFilter;
            }
            set { _archiveDateFilter = value; }
        }

        /// <summary>
        ///  Поле ФильтрДатыДокумента
        /// </summary>
        /// <value>
        /// ФильтрДатыДокумента (smallint, not null)
        /// </value>
        public static Int16 DateDocumentFilter
        {
            get
            {
                if (!_available) LoadDocViewParams();
                return _dateDocumentFilter;
            }
            set { _dateDocumentFilter = value; }
        }

        /// <summary>
        ///  Поле ФильтрДатыСоздания
        /// </summary>
        /// <value>
        /// ФильтрДатыСоздания (smallint, not null)
        /// </value>
        public static Int16 CreationDateFilter
        {
            get
            {
                if (!_available) LoadDocViewParams();
                return _creationDateFilter;
            }
            set { _creationDateFilter = value; }
        }

        /// <summary>
        ///  Поле ПодтвУдаления
        /// </summary>
        /// <value>
        /// ПодтвУдаления (bit, not null)
        /// </value>
        public static bool DeleteConfirm
        {
            get
            {
                if (!_available) LoadDocViewParams();
                return _deleteConfirm;
            }
            set { _deleteConfirm = value; }
        }

        /// <summary>
        ///  Поле ПодтвГрупповыхОпераций
        /// </summary>
        /// <value>
        /// ПодтвГрупповыхОпераций (bit, not null)
        /// </value>
        public static bool GroupOperationsConfirm
        {
            get
            {
                if (!_available) LoadDocViewParams();
                return _groupOperationsConfirm;
            }
            set { _groupOperationsConfirm = value; }
        }

        /// <summary>
        ///  Поле ПоказыватьНовости
        /// </summary>
        /// <value>
        /// ПоказыватьНовости (bit, not null)
        /// </value>
        public static bool ShowNews
        {
            get
            {
                if (!_available) LoadDocViewParams();
                return _showNews;
            }
            set { _showNews = value; }
        }

        /// <summary>
        ///  Поле СохранениеДобавитьВРаботу
        /// </summary>
        /// <value>
        /// СохранениеДобавитьВРаботу (bit, not null)
        /// </value>
        public static bool AddWorkSaving
        {
            get
            {
                if (!_available) LoadDocViewParams();
                return _addWorkSaving;
            }
            set { _addWorkSaving = value; }
        }

        /// <summary>
        ///  Поле СохранениеОткрытьСохранённый
        /// </summary>
        /// <value>
        /// СохранениеОткрытьСохранённый (bit, not null)
        /// </value>
        public static bool SavingOpenSaved
        {
            get
            {
                if (!_available) LoadDocViewParams();
                return _savingOpenSaved;
            }
            set { _savingOpenSaved = value; }
        }

        /// <summary>
        ///  Поле СохранениеПослатьСообщение
        /// </summary>
        /// <value>
        /// СохранениеПослатьСообщение (bit, not null)
        /// </value>
        public static bool SendMessageSaving
        {
            get
            {
                if (!_available) LoadDocViewParams();
                return _sendMessageSaving;
            }
            set { _sendMessageSaving = value; }
        }

        /// <summary>
        ///  Поле ПодписьВыполненоСообщение. 
        ///  Не кешируется, при обращении все время загружает актуальные данные из БД.
        /// </summary>
        /// <value>
        /// ПодписьВыполненоСообщение (bit, not null)
        /// </value>
        public static bool SignMessageWorkDone
        {
            get
            {
                LoadDocViewParams();
                return _signMessageWorkDone;
            }
            set { _signMessageWorkDone = value; }
        }

        /// <summary>
        ///  Поле ФаксыВходящиеТолькоНеСохранённые
        /// </summary>
        /// <value>
        /// ФаксыВходящиеТолькоНеСохранённые (bit, not null)
        /// </value>
        public static bool IncomingFaxesJustNotSaved
        {
            get
            {
                if (!_available) LoadDocViewParams();
                return _incomingFaxesJustNotSaved;
            }
            set { _incomingFaxesJustNotSaved = value; }
        }

        /// <summary>
        ///  Поле ФаксыОтправленныеТолькоНеСохранённые
        /// </summary>
        /// <value>
        /// ФаксыОтправленныеТолькоНеСохранённые (bit, not null)
        /// </value>
        public static bool SendFaxesJustNotSaved
        {
            get
            {
                if (!_available) LoadDocViewParams();
                return _sendFaxesJustNotSaved;
            }
            set { _sendFaxesJustNotSaved = value; }
        }

        /// <summary>
        ///  Поле ПереходНаСледующийПриОтправкеСообщения
        /// </summary>
        /// <value>
        /// ПереходНаСледующийПриОтправкеСообщения (bit, not null)
        /// </value>
        public static bool MoveNextSendingMessages
        {
            get
            {
                if (!_available) LoadDocViewParams();
                return _moveNextSendingMessages;
            }
            set { _moveNextSendingMessages = value; }
        }

        /// <summary>
        ///  Поле ИскатьНесколькоДокументовПоШтрихкоду
        /// </summary>
        /// <value>
        /// ИскатьНесколькоДокументовПоШтрихкоду (bit, not null)
        /// </value>
        public static bool SearchMultipleDocumentsBarcode
        {
            get
            {
                if (!_available) LoadDocViewParams();
                return _searchMultipleDocumentsBarcode;
            }
            set { _searchMultipleDocumentsBarcode = value; }
        }

        /// <summary>
        ///  Поле ЛичныеСпискиРассылкиПоказыватьПервыми
        /// </summary>
        /// <value>
        /// ЛичныеСпискиРассылкиПоказыватьПервыми (bit, not null)
        /// </value>
        public static bool PersonalSendListFirstOrder
        {
            get
            {
                if (!_available) LoadDocViewParams();
                return _personalSendListFirstOrder;
            }
            set { _personalSendListFirstOrder = value; }
        }

        /// <summary>
        ///  Поле ПрочитыватьСообщениеПриЗавершенииРаботы
        /// </summary>
        /// <value>
        /// ПрочитыватьСообщениеПриЗавершенииРаботы (tinyint, not null)
        /// </value>
        public static byte ReadMessageEndWork
        {
            get
            {
                if (!_available) LoadDocViewParams();
                return _readMessageEndWork;
            }
            set { _readMessageEndWork = value; }
        }

        #endregion

        /// <summary>
        ///  Обновить данные
        /// </summary>
        /// <remarks>
        ///  По сути является более уместным псевданимом для LoadDocViewParams()
        /// </remarks>
        public static void Refresh()
        {
            LoadDocViewParams();
        }

        /// <summary>
        /// Получение параметров
        /// </summary>
        private static void LoadDocViewParams()
        {
            using (var dbReader = new DBReader(SQLQueries.SELECT_НастройкиDocView, CommandType.Text, CN))
            {
                if (dbReader.HasRows)
                {
                    #region Получение порядкового номера столбца

                    int colКодЛица = dbReader.GetOrdinal("КодЛица");
                    int colПорядокГруппировки = dbReader.GetOrdinal("ПорядокГруппировки");
                    int colКодыДокументовСвязующих = dbReader.GetOrdinal("КодыДокументовСвязующих");
                    int colУведомлениеСообщения = dbReader.GetOrdinal("УведомлениеСообщения");
                    int colВремяОтметкиПрочтения = dbReader.GetOrdinal("ВремяОтметкиПрочтения");
                    int colДокументыПодуровней = dbReader.GetOrdinal("ДокументыПодуровней");
                    int colФильтрДатыАрхивирования = dbReader.GetOrdinal("ФильтрДатыАрхивирования");
                    int colФильтрДатыДокумента = dbReader.GetOrdinal("ФильтрДатыДокумента");
                    int colФильтрДатыСоздания = dbReader.GetOrdinal("ФильтрДатыСоздания");
                    int colПодтвУдаления = dbReader.GetOrdinal("ПодтвУдаления");
                    int colПодтвГрупповыхОпераций = dbReader.GetOrdinal("ПодтвГрупповыхОпераций");
                    int colПоказыватьНовости = dbReader.GetOrdinal("ПоказыватьНовости");
                    int colСохранениеДобавитьВРаботу = dbReader.GetOrdinal("СохранениеДобавитьВРаботу");
                    int colСохранениеОткрытьСохранённый = dbReader.GetOrdinal("СохранениеОткрытьСохранённый");
                    int colСохранениеПослатьСообщение = dbReader.GetOrdinal("СохранениеПослатьСообщение");
                    int colПодписьВыполненоСообщение = dbReader.GetOrdinal("ПодписьВыполненоСообщение");
                    int colФаксыВходящиеТолькоНеСохранённые = dbReader.GetOrdinal("ФаксыВходящиеТолькоНеСохранённые");
                    int colФаксыОтправленныеТолькоНеСохранённые = dbReader.GetOrdinal("ФаксыОтправленныеТолькоНеСохранённые");
                    int colПереходНаСледующийПриОтправкеСообщения = dbReader.GetOrdinal("ПереходНаСледующийПриОтправкеСообщения");
                    int colИскатьНесколькоДокументовПоШтрихкоду = dbReader.GetOrdinal("ИскатьНесколькоДокументовПоШтрихкоду");
                    int colЛичныеСпискиРассылкиПоказыватьПервыми = dbReader.GetOrdinal("ЛичныеСпискиРассылкиПоказыватьПервыми");
                    int colПрочитыватьСообщениеПриЗавершенииРаботы = dbReader.GetOrdinal("ПрочитыватьСообщениеПриЗавершенииРаботы");
                    #endregion

                    if (dbReader.Read())
                    {
                         _available = true;
                        _codePerson = !dbReader.IsDBNull(colКодЛица) ? dbReader.GetInt32(colКодЛица) : 0;
                        _orderGroup = dbReader.GetString(colПорядокГруппировки);
                        _codeDocumentLink = dbReader.GetString(colКодыДокументовСвязующих);
                        _messagesNotify = dbReader.GetBoolean(colУведомлениеСообщения);
                        _readMarkTime = dbReader.GetInt32(colВремяОтметкиПрочтения);
                        _sublevelDocuments = dbReader.GetBoolean(colДокументыПодуровней);
                        _archiveDateFilter = dbReader.GetInt16(colФильтрДатыАрхивирования);
                        _dateDocumentFilter = dbReader.GetInt16(colФильтрДатыДокумента);
                        _creationDateFilter = dbReader.GetInt16(colФильтрДатыСоздания);
                        _deleteConfirm = dbReader.GetBoolean(colПодтвУдаления);
                        _groupOperationsConfirm = dbReader.GetBoolean(colПодтвГрупповыхОпераций);
                        _showNews = dbReader.GetBoolean(colПоказыватьНовости);
                        _addWorkSaving = dbReader.GetBoolean(colСохранениеДобавитьВРаботу);
                        _savingOpenSaved = dbReader.GetBoolean(colСохранениеОткрытьСохранённый);
                        _sendMessageSaving = dbReader.GetBoolean(colСохранениеПослатьСообщение);
                        _signMessageWorkDone = dbReader.GetBoolean(colПодписьВыполненоСообщение);
                        _incomingFaxesJustNotSaved = dbReader.GetBoolean(colФаксыВходящиеТолькоНеСохранённые);
                        _sendFaxesJustNotSaved = dbReader.GetBoolean(colФаксыОтправленныеТолькоНеСохранённые);
                        _moveNextSendingMessages = dbReader.GetBoolean(colПереходНаСледующийПриОтправкеСообщения);
                        _searchMultipleDocumentsBarcode = dbReader.GetBoolean(colИскатьНесколькоДокументовПоШтрихкоду);
                        _personalSendListFirstOrder = dbReader.GetBoolean(colЛичныеСпискиРассылкиПоказыватьПервыми);
                        _readMessageEndWork = dbReader.GetByte(colПрочитыватьСообщениеПриЗавершенииРаботы);
                    }
                }
                else { _available = false; }
            }
        }

        /// <summary>
        /// Сохранение параметров
        /// </summary>
        public static void SaveDVParameters()
        {
            var sqlParams = new Dictionary<string, object>
            {
                {"@КодЛица", _codePerson},
                {"@ПорядокГруппировки", _orderGroup},
                {"@КодыДокументовСвязующих", _codeDocumentLink},
                {"@УведомлениеСообщения", _messagesNotify},
                {"@ВремяОтметкиПрочтения", _readMarkTime},
                {"@ДокументыПодуровней", _sublevelDocuments},
                {"@ФильтрДатыАрхивирования", _archiveDateFilter},
                {"@ФильтрДатыДокумента", _dateDocumentFilter},
                {"@ФильтрДатыСоздания", _creationDateFilter},
                {"@ПодтвУдаления", _deleteConfirm},
                {"@ПодтвГрупповыхОпераций", _groupOperationsConfirm},
                {"@ПоказыватьНовости", _showNews},
                {"@СохранениеДобавитьВРаботу", _addWorkSaving},
                {"@СохранениеОткрытьСохранённый", _savingOpenSaved},
                {"@СохранениеПослатьСообщение", _sendMessageSaving},
                {"@ПодписьВыполненоСообщение", _signMessageWorkDone},
                {"@ФаксыВходящиеТолькоНеСохранённые", _incomingFaxesJustNotSaved},
                {"@ФаксыОтправленныеТолькоНеСохранённые", _sendFaxesJustNotSaved},
                {"@ПереходНаСледующийПриОтправкеСообщения", _moveNextSendingMessages},
                {"@ИскатьНесколькоДокументовПоШтрихкоду", _searchMultipleDocumentsBarcode},
                {"@ЛичныеСпискиРассылкиПоказыватьПервыми", _personalSendListFirstOrder},
                {"@ПрочитыватьСообщениеПриЗавершенииРаботы", _readMessageEndWork}
            };

            DBManager.ExecuteNonQuery(SQLQueries.UPDATE_НастройкиDocView, CommandType.Text, CN, sqlParams);
        }
    }

}
