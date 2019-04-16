﻿namespace Kesco.Lib.Entities
{
    /// <summary>
    ///     SQLзапросы
    /// </summary>
    public static class SQLQueries
    {
        #region Текущие лица

        /// <summary>
        ///     Строка запроса: ТекущиеЛица
        /// </summary>
        public const string SELECT_ТекущиеЛица = @"
SELECT {0} T0.КодЛица, T0.Кличка
FROM vwЛица T0 (nolock)
WHERE EXISTS (SELECT * FROM Инвентаризация.dbo.fn_ТекущиеЛица() X WHERE КодЛица = T0.КодЛица)";

        #endregion

        #region Service

        /// <summary>
        ///     Запрос к функции fn_ReplaceRusLat
        /// </summary>
        public const string SELECT_FN_ReplaceRusLat = "SELECT @s = dbo.fn_ReplaceRusLat(@str)";

        /// <summary>
        ///     Строка запроса: Права доступа к таблице
        /// </summary>
        public const string SELECT_ПраваНаТаблицу = @"
SELECT
    HAS_PERMS_BY_NAME(@TableName, 'OBJECT', 'select') as PermOnSelect, 
    HAS_PERMS_BY_NAME(@TableName, 'OBJECT', 'insert') as PermOnInsert, 
    HAS_PERMS_BY_NAME(@TableName, 'OBJECT', 'update') as PermOnUpdate, 
    HAS_PERMS_BY_NAME(@TableName, 'OBJECT', 'delete') as PermOnDelete; 
";
 
        #endregion

        #region Ресурсы лица

        /// <summary>
        ///     Строка запроса: РесурсыЛица
        /// </summary>
        public const string SELECT_РесурсыЛица =
            @"SELECT * FROM РесурсыЛица (nolock) WHERE КодРесурса = @КодРесурса AND КодЛица = @КодЛица";

        #endregion

        #region БазисПоставки

        /// <summary>
        ///     Строка запроса: Получить Базис
        /// </summary>
        public const string SELECT_ID_БазисПоставки = @"
SELECT  T0.*, T1.ВидТранспорта 
FROM    БазисыПоставок T0 INNER JOIN ВидыТранспорта T1 ON T0.КодВидаТранспорта = T1.КодВидаТранспорта 
WHERE   КодБазисаПоставки = @Id";

        #endregion

        #region Отправки

        /// <summary>
        ///     Строка запроса: Отправки
        /// </summary>
        public const string DeliveryFieldsRequest = @"
IF object_id('tempdb..#СвойстваУчастка') IS NOT NULL DROP TABLE #СвойстваУчастка

CREATE TABLE #СвойстваУчастка (
	НомерДокумента nvarchar(50),
	КодДокумента int,
	КодГО int,
	УзелОтправления int,
	ОтметкиГО nvarchar(100),
	РеквизитыГО nvarchar(300),
	КодГП int,
	УзелНазначения int,
	ОтметкиГП nvarchar(100),
	РеквизитыГП nvarchar(300)
)

INSERT #СвойстваУчастка( НомерДокумента )
SELECT vwОтправкаВагоновУчастки.НомерДокумента
FROM ОтправкаВагоновВыгрузка (nolock)
	INNER JOIN vwОтправкаВагоновУчастки ON vwОтправкаВагоновУчастки.КодУчасткаОтправкиВагона = ОтправкаВагоновВыгрузка.КодОтправкиВагонаУчасток
WHERE guid = @gd

UPDATE #СвойстваУчастка SET КодДокумента = vwДокументы.КодДокумента
FROM #СвойстваУчастка
	INNER JOIN vwДокументы (nolock) ON КодТипаДокумента = 2144 AND vwДокументы.НомерДокумента = #СвойстваУчастка.НомерДокумента

-- Если существуют отправки, по которым были найдены ЖД накладные, ищем данные в Э.Ф.
IF EXISTS( SELECT * FROM #СвойстваУчастка WHERE КодДокумента IS NOT NULL )
BEGIN
	UPDATE #СвойстваУчастка SET
		КодГО = vwДокументыДанные.КодЛица3,
		УзелОтправления = vwДокументыДанные.КодТУзла1,
		ОтметкиГО = vwДокументыДанные.Text100_1,
		РеквизитыГО = vwДокументыДанные.Text300_1,
		КодГП = vwДокументыДанные.КодЛица4,
		УзелНазначения = vwДокументыДанные.КодТУзла2,
		ОтметкиГП = vwДокументыДанные.Text100_2,
		РеквизитыГП = vwДокументыДанные.Text300_2
	FROM #СвойстваУчастка
		INNER JOIN vwДокументыДанные (nolock) ON vwДокументыДанные.КодДокумента = #СвойстваУчастка.КодДокумента
END

SELECT DISTINCT КодГО, УзелОтправления, ОтметкиГО, РеквизитыГО, КодГП, УзелНазначения, ОтметкиГП, РеквизитыГП FROM #СвойстваУчастка

IF object_id('tempdb..#СвойстваУчастка') IS NOT NULL DROP TABLE #СвойстваУчастка";

        #endregion

        #region Рабочее место менеджера

        /// <summary>
        ///     Строка запроса: Договора Куратора
        /// </summary>
        public const string SELECT_ДоговораКуратора = @"
IF OBJECT_ID('tempdb..#Договора') IS NOT NULL   DROP TABLE #Договора
IF OBJECT_ID('tempdb..#Лица') IS NOT NULL       DROP TABLE #Лица

CREATE TABLE #Договора(	КодДоговора int PRIMARY KEY, НомерДоговора nvarchar(100), Договор nvarchar(300), НачалоДействияДоговора datetime,
			КодИсполнителя int, Исполнитель varchar(50), КодЗаказчика int, Заказчик varchar(50), Описание nvarchar(500),	
			ДеньПолученияДокументов int, МесяцПолученияДокументов int,
			ДеньОплаты int, МесяцОплаты int, АбонПлата money, ЕстьУсловия tinyint, БалансЗаказчика money, КодСотрудникаКуратора int)

CREATE TABLE #Лица(КодЛица int PRIMARY KEY, Кличка varchar(50))

INSERT	#Договора(КодДоговора, НомерДоговора, Договор, Описание, НачалоДействияДоговора, КодИсполнителя, КодЗаказчика, ЕстьУсловия, КодСотрудникаКуратора)
SELECT	КодДокумента, 
	    НомерДокумента,
	    CASE WHEN НазваниеДокумента <> '' THEN НазваниеДокумента ELSE ТипДокумента END +' №' + НомерДокумента + ' от ' + CONVERT(varchar,ДатаДокумента,104) Договор, 
	    Описание,
	    НачалоДействияДоговора,
	    КодИсполнителя,
	    КодЗаказчика,
	    0 ЕстьУсловия,
	    КодСотрудникаКуратора	
FROM(	SELECT	КодДокумента, КодТипаДокумента, НазваниеДокумента, ДатаДокумента, НомерДокумента, Описание,
		        КодЛица1 КодИсполнителя,
		        КодЛица2 КодЗаказчика,
		        Дата2 НачалоДействияДоговора,
		        КодСотрудника1 КодСотрудникаКуратора
	    FROM vwДокументыДокументыДанные ДД (nolock)
	    WHERE ДД.КодСотрудника1 = @КодКуратора ) X INNER JOIN 
	    ТипыДокументов ON X.КодТипаДокумента = ТипыДокументов.КодТипаДокумента
WHERE	EXISTS(SELECT * FROM ТипыДокументов Parent WHERE Parent.КодТипаДокумента=2039 AND Parent.L<=ТипыДокументов.L AND Parent.R>=ТипыДокументов.R) --AND КодДокумента=3019449

--удаляем из списка договора, работа по которым завершена				
DELETE FROM #Договора WHERE EXISTS(SELECT * FROM ПодписиДокументов WHERE КодДокумента=#Договора.КодДоговора AND ТипПодписи=1)

-- Получим названия лиц.
INSERT	#Лица
SELECT	КодЛица, Кличка 
FROM	Справочники.dbo.vwЛица Лица (nolock)
WHERE	КодЛица IN (SELECT КодИсполнителя FROM #Договора UNION SELECT КодЗаказчика FROM #Договора)

UPDATE	X
SET	    Исполнитель = CASE WHEN Лица.Кличка IS NULL THEN '#' + CONVERT(varchar,X.КодИсполнителя) ELSE Лица.Кличка  END
FROM	#Договора X LEFT JOIN #Лица Лица (nolock) ON X.КодИсполнителя=Лица.КодЛица

UPDATE	X
SET	    Заказчик = CASE WHEN Лица.Кличка IS NULL THEN '#' + CONVERT(varchar,X.КодЗаказчика) ELSE Лица.Кличка  END
FROM	#Договора X LEFT JOIN #Лица Лица (nolock) ON X.КодЗаказчика=Лица.КодЛица

--Получаем условия договоров
UPDATE	X
SET     ДеньПолученияДокументов		= Y.ДеньМесяцаПолученияДокументов%100,
	    МесяцПолученияДокументов	= Y.ДеньМесяцаПолученияДокументов/100-2,
	    ДеньОплаты			        = Y.ДеньМесяцаОплатыПоДоговору%100,
    	МесяцОплаты			        = Y.ДеньМесяцаОплатыПоДоговору/100-2,
	    АбонПлата			        = Y.СуммаАбонентскойПлаты,
	    ЕстьУсловия			        = 1
FROM	#Договора X INNER JOIN 
	    ДокументыДоговораОказанияУслуг Y ON X.КодДоговора = Y.КодДокументаДоговораОказанияУслуг

SELECT  КодДоговора [Код договора], НомерДоговора Номер, Договор, НачалоДействияДоговора [Начало действия],
	    КодИсполнителя [Код исполнителя], Исполнитель, КодЗаказчика [Код заказчика], Заказчик, Описание,	
		АбонПлата, ЕстьУсловия [Есть условия], БалансЗаказчика Баланс, КодСотрудникаКуратора [Код куратора]
FROM #Договора

";

        #endregion

        #region ТарификацияСотовых

        /// <summary>
        ///     Строка запроса: ТарификацияСотовых за месяц
        /// </summary>
        public const string SELECT_ТарификацияСотовых = @"
SELECT TOP 60000
            НачалоРазговора, Телефон, Абонент,
            CASE WHEN Исходящий = 1 THEN 'Исх.' ELSE 'Вх.' END Тип, 
            Услуга,
            LTRIM(ЗонаАбонента + ' ' + Направление) Описание,
            Секунд, Килобайт, 
            CASE Роуминг WHEN 1 THEN 'МГ' WHEN 2 THEN 'МН' ELSE '-' END Роуминг,
		        Сумма,
		        СуммаСотрудника		
    FROM vwТарификацияСотовых Т (nolock)
WHERE Год = @Год AND Т.Месяц=@Месяц
ORDER BY    КодТарификацииСотовых DESC";

        #endregion

        #region TreeView

        /// <summary>
        ///     Строка запроса: Загрузить узлы дерева (если не заполнена строка поиска и не выбраны узлы)
        /// </summary>
        public const string SELECT_ЗагрузкаУзловДерева = @"
SELECT {1} Id, r.R-L ЕстьДети, {2} Text, ISNULL(r.Parent,0) ParentId, '' Фильтр
FROM {0} r
WHERE   ((@Потомки = 1 AND ((@Код = 0 AND r.Parent IS NULL) OR (@Код <> 0 AND r.Parent = @Код)))
        OR (@Потомки = 0 AND {1} = @Код))                     
ORDER BY r.L
";

        /// <summary>
        ///     Строка запроса: Получить список открытых узлов дерева
        /// </summary>
        public const string SELECT_ПолучениеОткрытыхУзловДерева = @"
SELECT T0.{1} FROM {0} T0, {0} T1 WHERE T1.{1} = @id AND T1.L BETWEEN T0.L AND T0.R AND T0.{1} <> @id ORDER BY T0.L
";

        /// <summary>
        ///     Строка запроса: Получить полный путь к узлу дерева с помощью функции
        /// </summary>
        public const string SELECT_ПолныйПутьКУзлуДереваФункция = @"
DECLARE  @sql nvarchar(500);
SET @sql = N'SELECT ' + @funcname + '(@Code, @PathType) Path';
exec sp_executesql @sql, N'@Code int, @PathType bit', @Code, @PathType;
";

        /// <summary>
        ///     Строка запроса: Получить полный путь к узлу дерева из поля
        /// </summary>
        public const string SELECT_ПолныйПутьКУзлуДерева = @"
SELECT {2}{3} FROM {0} WHERE {1} = @id
";


        /// <summary>
        ///     Строка запроса: Переместить узел дерева со сменой родительского узла
        /// </summary>
        public const string UPDATE_ПеремещениеУзлаДерева = @"
UPDATE {0} SET Parent = @Parent WHERE {1} = @Id
";

        /// <summary>
        ///     Строка запроса: Переместить узел дерева без смены родительского узла
        /// </summary>
        public const string UPDATE_ПеремещениеУзлаДереваБезСменыРодителя = @"
DECLARE @L int, @L1 int, @OLD_ID_ON_NEW_POSITION int
SELECT @L1 = L FROM {1} WHERE {2} = @Id
SELECT @L = L, @OLD_ID_ON_NEW_POSITION = {2} FROM(
    SELECT row_number() OVER (ORDER BY L ASC) AS rownumber, * FROM {1} WHERE(@Parent<> 0 AND Parent = @Parent) OR @Parent = 0 AND Parent IS NULL) RN
WHERE rownumber = @NewPosition+1
UPDATE {0} SET L = @L WHERE {2} = @Id
IF(@L1<@L)
BEGIN
    SELECT @L=L FROM {1} WHERE {2} = @Id
    UPDATE {0} SET L = @L WHERE {2} = @OLD_ID_ON_NEW_POSITION
END
";

        /// <summary>
        ///     Строка запроса: Добавить узел дерева 
        /// </summary>
        public const string INSERT_ДобавлениеУзлаДерева = @"
INSERT {0} ({1}, Parent) VALUES (@name, {2})
";

        /// <summary>
        ///     Строка запроса: Редактировать узел дерева 
        /// </summary>
        public const string UPDATE_РедактированиеУзлаДерева = @"
UPDATE {0} SET {2} = @name WHERE {1} = @id
";

        /// <summary>
        ///     Строка запроса: Удалить узел дерева 
        /// </summary>
        public const string UPDATE_УдалениеУзлаДерева = @"
DELETE FROM {0} WHERE {1} = @Id
";

        #endregion

        //++++++++++++++++++ CORPORATE ++++++++++++++++++

        #region Сотрудники

        /// <summary>
        ///     Строка запроса:  Получить список сотрудников
        /// </summary>
        public const string SELECT_ВсеСотрудники = @"SELECT * FROM Сотрудники (nolock) ";

        /// <summary>
        ///     Строка запроса: Получить Данные Сотрудника
        /// </summary>
        public static string SELECT_ID_Сотрудник = string.Format(@"{0}WHERE КодСотрудника=@Id", SELECT_ВсеСотрудники);

        /// <summary>
        ///     Строка запроса: Получить Список Текущих Сотрудников
        /// </summary>
        public const string SELECT_FN_ТекущийСотрудник = @"SELECT КодСотрудника FROM fn_ТекущийСотрудник() X";

        /// <summary>
        ///     Строка запроса:  Получить Данные Текущего Сотрудника
        /// </summary>
        public const string SELECT_SID_ТекущийСотрудник = @"SELECT * FROM Сотрудники (nolock) WHERE SID = SUSER_SID()";

        /// <summary>
        ///     Строка запроса: Получаем подразделение сотрудника
        /// </summary>
        public const string SELECT_ПодразделениеСотрудника = @"
SELECT TOP 1 Child.Подразделение 
FROM vwДолжности Child (nolock) INNER JOIN vwДолжности Parent (nolock) ON Child.L <= Parent.L AND Child.R >= Parent.R 
WHERE Parent.КодСотрудника = @Id AND Child.Подразделение <> '' ORDER BY Child.R";

        /// <summary>
        ///     Строка запроса: Получить Данные общих сотрудников
        /// </summary>
        public static string SELECT_ОбщихСотрудников =
            string.Format(
                @"{0}WHERE КодОбщегоСотрудника = @Id AND КодСотрудника <> @Id AND Состояние = 0 ORDER BY Сотрудник",
                SELECT_ВсеСотрудники);

        /// <summary>
        ///     Строка запроса: Получить сотрудников кроме указанного, имееющих рабочее место на указанном расположении
        /// </summary>
        public static string SELECT_ДругиеСотрудникиНаРасположении = string.Format(@"
{0} INNER JOIN 
РабочиеМеста (nolock) ON Сотрудники.КодСотрудника = РабочиеМеста.КодСотрудника
WHERE РабочиеМеста.КодРасположения = @КодРасположения AND РабочиеМеста.КодСотрудника <> @КодСотрудника AND Сотрудники.Состояние = 0
ORDER BY Сотрудник", SELECT_ВсеСотрудники);

        /// <summary>
        ///     Строка запроса: Удалить замещение
        /// </summary>
        public static string DELETE_ID_ЗамещениеСотрудника = @"
UPDATE ЗамещенияСотрудников
SET До = CASE WHEN От > getdate() THEN ОТ ELSE FLOOR(CONVERT(float, getdate())) END
WHERE КодЗамещенияСотрудников = @Id";

        /// <summary>
        ///     Строка запроса: Получить Замещения сотрудника по замещаемому
        /// </summary>
        public static string SELECT_ЗамещенияСотрудника_ПоЗамещаемому = @"
SELECT  КодЗамещенияСотрудников,
	    До,
	    КодСотрудникаЗамещаемого,
	    Замещённый.ФИО Замещённый,
	    КодСотрудникаЗамещающего,
	    ИспОбязанности.ФИО ИспОбязанности,
	    ЗамещенияСотрудников.Примечания,
	    Изменил.ФИО Изменил,
	    ЗамещенияСотрудников.Изменено
FROM    ЗамещенияСотрудников INNER JOIN 
        Сотрудники Замещённый ON ЗамещенияСотрудников.КодСотрудникаЗамещаемого = Замещённый.КодСотрудника INNER JOIN 
        Сотрудники ИспОбязанности ON ЗамещенияСотрудников.КодСотрудникаЗамещающего = ИспОбязанности.КодСотрудника INNER JOIN 
        Сотрудники Изменил ON ЗамещенияСотрудников.Изменил = Изменил.КодСотрудника
WHERE   КодСотрудникаЗамещаемого = @Id AND До > getdate() AND До <>От";

        /// <summary>
        ///     Строка запроса: Получить Замещения сотрудника по замещающему
        /// </summary>
        public static string SELECT_ЗамещенияСотрудника_ПоЗамещающему = @"
SELECT  КодЗамещенияСотрудников,
	    До,
	    КодСотрудникаЗамещаемого,
	    Замещённый.ФИО Замещённый,
	    КодСотрудникаЗамещающего,
	    ИспОбязанности.ФИО ИспОбязанности,
	    ЗамещенияСотрудников.Примечания,
	    Изменил.ФИО Изменил,
	    ЗамещенияСотрудников.Изменено
FROM    ЗамещенияСотрудников INNER JOIN 
        Сотрудники Замещённый ON ЗамещенияСотрудников.КодСотрудникаЗамещаемого = Замещённый.КодСотрудника INNER JOIN
        Сотрудники ИспОбязанности ON ЗамещенияСотрудников.КодСотрудникаЗамещающего = ИспОбязанности.КодСотрудника INNER JOIN 
        Сотрудники Изменил ON ЗамещенияСотрудников.Изменил = Изменил.КодСотрудника
WHERE   КодСотрудникаЗамещающего = @Id AND До > getdate() AND До<>От";

        /// <summary>
        ///     Строка запроса: Получить Данные о последнем проходе сотрудника
        /// </summary>
        public static string SELECT_ПоследнийПроходСотрудника = @" 
SELECT Считыватель, ПоследнийПроход
FROM Инвентаризация.dbo.ПоследнийПроходСотрудников 
WHERE КодСотрудника = @КодСотрудника";

        /// <summary>
        ///     Строка запроса: Проверить наличие административных прав в компаниях сотрудника
        /// </summary>
        public static string SELECT_АдминистраторСотрудника = @" 
DECLARE @ЯвляетсяАдминистраторомСотрудника bit 
SET @ЯвляетсяАдминистраторомСотрудника = 0

SELECT  @ЯвляетсяАдминистраторомСотрудника = 1 
FROM    dbo.fn_ТекущиеРоли() X 
WHERE	КодРоли IN (31, 32, 43) AND (КодЛица = 0 OR КодЛица IN (SELECT КодЛица FROM dbo.fn_КомпанииСотрудника(@Id)))
			
SELECT @ЯвляетсяАдминистраторомСотрудника ЯвляетсяАдминистраторомСотрудника";

        /// <summary>
        ///     Поулчение информации из vwADSI_AllUsers по логину
        /// </summary>
        public const string SELECT_ADSI_ПоЛогину = @"
SELECT * FROM vwADSI_AllUsers WHERE Login=@Login
";
        /// <summary>
        ///     Поулчение информации из vwADSI_AllUsers по коду сотрудника
        /// </summary>
        public const string SELECT_ADSI_ПоКодуСотрудника = @"
SELECT  * 
FROM    vwADSI_AllUsers X 
WHERE	EXISTS(SELECT * FROM Сотрудники WHERE КодСотрудника = @КодСотрудника AND (Сотрудники.GUID = X.GUID OR Сотрудники.[SID] = X.[SID]))
";

        /// <summary>
        ///     Хранимая процедура: Получить Контакты сотрудника
        /// </summary>
        public const string SP_Сотрудники_Контакты = "sp_Сотрудники_Контакты";

        /// <summary>
        ///     Строка запроса: Получить Получить лицо заказчика
        /// </summary>
        public static string SELECT_ЛицоЗаказчикаСотрудника = @" 
SELECT  ЛицаЗаказчики.*
FROM    Сотрудники INNER JOIN 
        ЛицаЗаказчики ON ЛицаЗаказчики.КодЛица = Сотрудники.КодЛицаЗаказчика
WHERE   Сотрудники.КодСотрудника = @Id";

        /// <summary>
        ///     Строка запроса: Получить рабочие места сотрудника
        /// </summary>
        public static string SELECT_РабочиеМестаСотрудника = @" 
SELECT РабочиеМеста.КодРасположения
      ,ISNULL(Расположение,'#'+CONVERT(varchar,РабочиеМеста.КодРасположения)) Расположение
      ,РасположениеRL
      ,ЧасовойПояс
      ,Офис
      ,ЛичныеПапки
      ,КодНомернойЁмкости
      ,CCM_Pool
      ,ISNULL(РабочееМесто,0) РабочееМесто
      ,Ключи
      ,Закрыто
      ,РасположениеPath0
      ,ISNULL(РасположениеPath1,'#'+CONVERT(varchar,РабочиеМеста.КодРасположения)) РасположениеPath1
      ,Parent
      ,L
      ,R
      ,РабочиеМеста.Изменил
      ,РабочиеМеста.Изменено      
FROM    РабочиеМеста LEFT JOIN 
        vwРасположения ON РабочиеМеста.КодРасположения = vwРасположения.КодРасположения
WHERE   РабочиеМеста.КодСотрудника = @id
ORDER BY L";

        /// <summary>
        ///     Сотрчудники на рабочем месте, кроме указанного
        /// </summary>
        public static string SELECT_СотрудникиНаРабочемМесте = @" 
SELECT  Сотрудники.*
FROM    РабочиеМеста INNER JOIN 
        Сотрудники ON Сотрудники.КодСотрудника = РабочиеМеста.КодСотрудника 
WHERE   РабочиеМеста.КодРасположения = @id 
        AND (@idEmpl = 0 OR РабочиеМеста.КодСотрудника <> @idEmpl) 
        AND (@state = -1 OR Сотрудники.Состояние = @state)
ORDER BY Сотрудники.Сотрудник
";

        /// <summary>
        ///     Строка запроса: ПроверкаПодчиненияСотрудника
        /// </summary>
        public const string SELECT_ПроверкаПодчиненияСотрудника = @"
SELECT Child.КодДолжности
FROM	dbo.vwДолжности Parent
		INNER JOIN dbo.vwДолжности Child ON Parent.L <= Child.L AND Parent.R >= Child.R
WHERE 	(Child.КодСотрудника=@КодСотрудника AND Parent.КодСотрудника=@КодРуководителя)OR
	    (Child.КодСотрудника=@КодСотрудника AND
		(EXISTS(SELECT Slave.*
			FROM dbo.ПодчинениеАдминистративное Chief
				INNER JOIN dbo.ПодчинениеАдминистративное Slave ON Chief.L <= Slave.L AND Chief.R >= Slave.R
				INNER JOIN dbo.vwДолжности D ON Chief.КодДолжности = D.КодДолжности
			WHERE Slave.КодДолжности=Parent.КодДолжности AND D.КодСотрудника=@КодРуководителя
			)
		)
	)";

        /// <summary>
        ///     Строка запроса: ФотографииСотрудника по его коду
        /// </summary>
        public static string SELECT_ФотографииСотрудника =
            "SELECT * FROM ФотографииСотрудников WHERE КодСотрудника = @id";

        /// <summary>
        ///     Строка запроса: Получить сотрудников, работающих совместно на рабочем месте
        /// </summary>
        public static string SELECT_СовместнаяРаботаНаРабочемМесте = @" 
SELECT  Сотрудники.КодСотрудника, Сотрудники.Сотрудник, Сотрудники.Employee 
FROM    РабочиеМеста INNER JOIN 
        Сотрудники ON Сотрудники.КодСотрудника = РабочиеМеста.КодСотрудника 
WHERE   РабочиеМеста.КодРасположения = @КодРасположения AND РабочиеМеста.КодСотрудника <> @КодСотрудника AND Сотрудники.Состояние = 0 AND Сотрудники.КодЛица IS NOT NULL
ORDER BY Сотрудники.Сотрудник";

        /// <summary>
        ///     Строка запроса: Получить сотрудников, работающих посменно на рабочем месте
        /// </summary>
        public static string SELECT_СотрудниковНаПосменнойРаботе = @"
DECLARE @СписокСотрудников varchar(1000)

SELECT  @СписокСотрудников = COALESCE(@СписокСотрудников + ', ', '') + Сотрудники.Сотрудник 
FROM    РабочиеМеста INNER JOIN 
        Сотрудники ON Сотрудники.КодСотрудника = РабочиеМеста.КодСотрудника INNER JOIN 
        vwРасположения ON vwРасположения.КодРасположения = РабочиеМеста.КодРасположения
WHERE   РабочиеМеста.КодРасположения = @КодРасположения AND РабочиеМеста.КодСотрудника <> @КодСотрудника AND Сотрудники.Состояние = 0 AND КодЛица IS NOT NULL AND vwРасположения.РабочееМесто = 1
ORDER BY Сотрудники.Сотрудник

SELECT @СписокСотрудников СписокСотрудников";

        /// <summary>
        ///     Строка запроса: Получить руководителя сотрудника --НЕ ИСПОЛЬЗОВАТЬ--
        /// </summary>
        public static string SELECT_ДанныеРуководителяСотрудника = @"
SELECT  ДолжностиСотрудников.КодЛица КодОрганизацииСотрудника, 
	    Сотрудники.КодСотрудника, Сотрудники.КодЛица КодЛицаСотрудника, Сотрудники.Сотрудник, ДолжностиСотрудников.Должность ДолжностьСотрудника,
        Руководитель.КодСотрудника КодСотрудникаРуководителя, Руководитель.КодЛица КодЛицаРуководителя, Руководитель.ФИО Руководитель, ДолжностиРуководителей.Должность ДолжностьРуководителя
FROM    Сотрудники INNER JOIN 
        vwДолжности ДолжностиСотрудников ON Сотрудники.КодСотрудника = ДолжностиСотрудников.КодСотрудника INNER JOIN 
        vwДолжности ДолжностиРуководителей ON ДолжностиСотрудников.Parent = ДолжностиРуководителей.КодДолжности LEFT JOIN 
        Сотрудники Руководитель ON ДолжностиРуководителей.КодСотрудника = Руководитель.КодСотрудника
WHERE	Сотрудники.КодСотрудника = @КодСотрудника 
	    AND (@Совместитель = -1 OR ДолжностиСотрудников.Совместитель = @Совместитель)
        AND (@ТолькоПоЛицуЗаказчику = 0 OR ДолжностиСотрудников.КодЛица = @КодЛицаЗаказчика)";

        /// <summary>
        ///     Строка запроса: Получение непостредственного руководителя
        /// </summary>
        public const string SELECT_НепосредственныйРуководитель = @"
SELECT dbo.fn_НепосредственныйРуководитель(@КодСотрудника) Руководитель
";

        /// <summary>
        ///     Строка запроса: Получение непостредственного руководителя
        /// </summary>
        public const string SELECT_НепосредственныйРуководитель_Данные = @"
SELECT * 
FROM	(SELECT	Сотрудники.КодЛица КодЛицаСотрудника, vwДолжности.КодЛица КодЛицаКомпанииСотрудника, vwДолжности.Должность ДолжностьСотрудника,
		        vwДолжности.Совместитель СотрудникСовместитель
	    FROM	Сотрудники LEFT JOIN 
		        vwДолжности ON Сотрудники.КодСотрудника = vwДолжности.КодСотрудника
	    WHERE	Сотрудники.КодСотрудника = @КодСотрудника) X CROSS JOIN 
	    (SELECT Сотрудники.КодСотрудника КодРуководителя, Сотрудники.Сотрудник Руководитель, Сотрудники.Employee РуководительЛат,
                Сотрудники.Дополнение, Сотрудники.Addition, Сотрудники.КодЛица КодЛицаРуководителя, vwДолжности.КодЛица КодЛицаКомпанииРуководителя,
		        ЛицаЗаказчики.КраткоеНазваниеРус НазваниеКомпанииРуководителя, ЛицаЗаказчики.КраткоеНазваниеЛат НазваниеКомпанииРуководителяЛат,
		        vwДолжности.Должность ДолжностьРуководителя, vwДолжности.Совместитель РуководительСовместитель
	    FROM	Сотрудники LEFT JOIN 
		        vwДолжности ON Сотрудники.КодСотрудника = vwДолжности.КодСотрудника LEFT JOIN
		        ЛицаЗаказчики ON vwДолжности.КодЛица = ЛицаЗаказчики.КодЛица
	    WHERE	Сотрудники.КодСотрудника = dbo.fn_НепосредственныйРуководитель(@КодСотрудника)) Y";

        /// <summary>
        ///     Строка запроса: Получение списка языков
        /// </summary>
        public const string SELECT_Языки = "SELECT * FROM Языки ORDER BY Язык";

        /// <summary>
        ///     Строка запроса: Получение языка
        /// </summary>
        public const string SELECT_ID_Язык = "SELECT * FROM Языки WHERE Язык = @id";

        /// <summary>
        ///     Строка запроса: Получение списка доменных имен
        /// </summary>
        public const string SELECT_DomainNames = "SELECT * FROM DomainNames ORDER BY DomainName";

        /// <summary>
        ///     Строка запроса: Получение доменного имени
        /// </summary>
        public const string SELECT_ID_DomainName = "SELECT * FROM DomainNames WHERE DomainName = @id";


        /// <summary>
        ///     Строка запроса: Получение списка общих папок
        /// </summary>
        public const string SELECT_CommonFolders = "SELECT * FROM ОбщиеПапки ORDER BY ОбщаяПапка";

        /// <summary>
        ///     Строка запроса: Получение общей папки
        /// </summary>
        public const string SELECT_ID_CommonFolder = "SELECT * FROM ОбщиеПапки WHERE КодОбщейПапки = @id";

        /// <summary>
        ///     Строка запроса: Получение общих папок, доступных сотруднику
        /// </summary>
        public const string SP_ОбщиеПапкиСотрудника = "sp_ОбщиеПапкиСотрудника";

        /// <summary>
        ///     Получение списка дополнительных прав
        /// </summary>
        public const string SELECT_AdvancedGrants = "SELECT * FROM ПраваДляУказанийIT ORDER BY Описание";

        /// <summary>
        ///     Строка запроса:  Получение дополнительного права
        /// </summary>
        public const string SELECT_ID_AdvancedGrant =
            "SELECT * FROM ПраваДляУказанийIT WHERE КодПраваДляУказанийIT = @id";

        /// <summary>
        ///     Строка запроса: Получение для сотрудника прав на типы лиц
        /// </summary>
        public const string SELECT_ПраваТипыЛицСотрудника = @"
SELECT	КодПраваТипыЛиц, 
        ПраваТипыЛиц.КодСотрудника, 
        ПраваТипыЛиц.КодКаталога, 
        CASE WHEN ПраваТипыЛиц.КодКаталога IS NOT NULL AND Каталоги.КодКаталога IS NULL THEN '#' + CONVERT(varchar,ПраваТипыЛиц.КодКаталога) ELSE Каталоги.Каталог END Каталог, 
        ПраваТипыЛиц.КодТемыЛица,
        CASE WHEN ПраваТипыЛиц.КодТемыЛица IS NOT NULL AND vwТемыЛиц.КодТемыЛица IS NULL THEN '#' + CONVERT(varchar,ПраваТипыЛиц.КодТемыЛица) ELSE vwТемыЛиц.ТемаЛица END ТемаЛица,  
        МожетДаватьПрава 
FROM	ПраваТипыЛиц (nolock) LEFT JOIN 
	    Каталоги ON ПраваТипыЛиц.КодКаталога = Каталоги.КодКаталога LEFT JOIN
	    vwТемыЛиц ON ПраваТипыЛиц.КодТемыЛица = vwТемыЛиц.КодТемыЛица
WHERE КодСотрудника = @КодСотрудника";

        /// <summary>
        ///     Строка запроса: Получение для сотрудника свободного логина
        /// </summary>
        public const string SELECT_СвободныйЛогин = @"
DECLARE @LName varchar(50), @FName varchar(50), @MName varchar(50), @Login varchar(50), @F tinyint

SELECT @LName=LastName, @FName=FirstName, @MName=MiddleName, @Login = Login FROM Сотрудники WHERE КодСотрудника=@КодСотрудника

IF ISNULL(@Login,'') = ''
BEGIN
    SET @Login=@LName
    SET @F=0

    WHILE EXISTS(SELECT * FROM Сотрудники WHERE Login=@Login AND КодСотрудника !=@КодСотрудника)
    BEGIN
	    SET @Login= LEFT(@FName,1)+ CASE WHEN @F=1 THEN LEFT(@MName,1) ELSE '' END +  @LName	
	    SET @F=@F+1	
	    IF @F>1 BEGIN SET @Login = '' BREAK	END
    END
END
SELECT LOWER(@Login) Login
";

        #endregion

        #region Лица заказчики

        /// <summary>
        ///     Строка запроса: Получение лиц заказчиков
        /// </summary>
        public const string SELECT_ЛицаЗаказчики = @"
SELECT {0} T0.КодЛица, T0.Кличка, КраткоеНазваниеЛат = CASE T0.КраткоеНазваниеЛат WHEN '' THEN T0.Кличка ELSE T0.КраткоеНазваниеЛат END 
FROM ЛицаЗаказчики T0 (nolock)
WHERE EXISTS (SELECT * FROM dbo.fn_ТекущиеЛица() X WHERE КодЛица = T0.КодЛица)";


        /// <summary>
        ///     Строка запроса: Поиск лица заказчика по ID
        /// </summary>
        public const string SELECT_ID_ЛицоЗаказчик = @"
SELECT КодЛица, Кличка, КраткоеНазваниеЛат = CASE КраткоеНазваниеЛат WHEN '' THEN Кличка ELSE КраткоеНазваниеЛат END 
FROM ЛицаЗаказчики (nolock) WHERE КодЛица = @id";

        #endregion

        #region Расположения

        /// <summary>
        ///     Строка запроса: Получение расположений
        /// </summary>
        public const string SELECT_Расположения = @"
SELECT {0} T0.КодРасположения, T0.Расположение, T0.РабочееМесто, T0.Офис, T0.Закрыто, T0.РасположениеPath0, T0.РасположениеPath1, T0.Parent, T0.L, T0.R
FROM dbo.vwРасположения T0 (nolock)";

        /// <summary>
        ///     Строка запроса: Поиск места хранения по коду
        /// </summary>
        public static string SELECT_РасположениеПоID =
            string.Format(@"{0} WHERE КодРасположения = @id", string.Format(SELECT_Расположения, ""));

        /// <summary>
        ///     Строка запроса: Все родители
        /// </summary>
        public static string SELECT_Родители = @"
SELECT T0.КодРасположения FROM dbo.vwРасположения T0, vwРасположения T1 WHERE T1.Кодрасположения = @id AND T1.L BETWEEN T0.L AND T0.R AND T0.Кодрасположения <> @id ORDER BY T0.L";

        /// <summary>
        ///     Строка запроса: Получение подчиненных расположений
        /// </summary>
        public static string SELECT_РасположенияПодчиненные = string.Format(
            @"{0} WHERE T0.L >= @leftKey AND T0.R <= @rightKey ORDER BY L", string.Format(SELECT_Расположения, ""));

        /// <summary>
        ///     Строка запроса: Проверка, что переданное расположение находится в офисе
        /// </summary>
        public static string SELECT_РасположениеВОфисе = @"
SELECT  Parent.КодРасположения, Parent.Расположение
FROM 	vwРасположения Parent INNER JOIN 
        vwРасположения Child ON Parent.L<=Child.L AND Parent.R>=Child.R
WHERE   Child.КодРасположения = @КодРасположения AND Parent.Офис <> 0";

        /// <summary>
        ///     Строка запроса: Проверка, организовано ли рабочее место
        /// </summary>
        public static string SELECT_РасположенияОрганизованыРабочиеМеста = @"
SELECT  *
FROM 	vwРасположенияОрганизованыРабочиеМеста 		
WHERE   КодРасположения = @КодРасположения";

        /// <summary>
        ///     Получение информации о расположениях для дерева расположений
        /// </summary>
        public static string SELECT_РасположенияДанныеДляДерева = @"
SELECT  r.КодРасположения Id, r.R-L ЕстьДети, r.Расположение Text, ISNULL(r.Parent,0) ParentId, r.Офис Office, r.РабочееМесто WorkPlace, '' Фильтр,
        ISNULL(x.ЕстьСотрудники,0) ЕстьСотрудники, ISNULL(rr.ЕстьОборудование,0) ЕстьОборудование
FROM    vwРасположения r 
LEFT JOIN 
        (SELECT COUNT(vwРеальноеРасположениеОборудования.КодРасположения) ЕстьОборудование, vwРеальноеРасположениеОборудования.КодРасположения
		FROM    vwРеальноеРасположениеОборудования
		GROUP BY КодРасположения) rr ON r.КодРасположения = rr.КодРасположения
LEFT JOIN 
        (SELECT COUNT(РабочиеМеста.КодРасположения) ЕстьСотрудники,РабочиеМеста.КодРасположения
		FROM    РабочиеМеста INNER JOIN 
                Сотрудники ON РабочиеМеста.КодСотрудника = Сотрудники.КодСотрудника
		WHERE   Сотрудники.Состояние = 0
		GROUP BY КодРасположения) x ON r.КодРасположения = x.КодРасположения
WHERE   ((@Потомки = 1 AND ((@Код = 0 AND r.Parent IS NULL) OR (@Код <> 0 AND r.Parent = @Код)))
        OR (@Потомки = 0 AND r.КодРасположения = @Код)) AND r.Закрыто=0                        
ORDER BY r.L
";


        /// <summary>
        ///     Получение информации для дерева
        /// </summary>
        public static string SELECT_РасположенияДанныеДляДерева_State = @"
SET NOCOUNT ON

IF OBJECT_ID('tempdb.#Расположения') IS NOT NULL DROP TABLE #Расположения
CREATE TABLE #Расположения(
        КодЗаписи int IDENTITY(1,1),
        [КодРасположения] [int],
        Расположение [varchar](300),       
        [Parent] [int],
        [L] [int],
        [R] [int],
        [Изменил] [int],
        [Изменено] [datetime],  
        BitMask tinyint
)

INSERT #Расположения
SELECT	Parent.[КодРасположения],
        Parent.Расположение,      
        Parent.[Parent],
        Parent.[L],
        Parent.[R],
        Parent.[Изменил],
        Parent.[Изменено], 
        1 BitMask
FROM	vwРасположения Parent 
WHERE EXISTS(SELECT * FROM vwРасположения Child 
					WHERE	Child.КодРасположения IN ({1})
						AND Parent.L <=	Child.L AND Parent.R>=Child.R) AND Закрыто=0
      AND NOT EXISTS(SELECT * FROM #Расположения X WHERE Parent.КодРасположения = X.КодРасположения)
ORDER BY Parent.L

INSERT #Расположения
SELECT	Child.[КодРасположения],
        Child.Расположение,      
        Child.[Parent],
        Child.[L],
        Child.[R],
        Child.[Изменил],
        Child.[Изменено], 
        2 BitMask
FROM	vwРасположения Parent
LEFT JOIN vwРасположения Child ON Child.Parent = Parent.КодРасположения
WHERE Parent.КодРасположения IN ({1}) AND Child.Закрыто=0 AND NOT EXISTS(SELECT * FROM #Расположения X WHERE Child.КодРасположения = X.КодРасположения)
ORDER BY Parent.L

INSERT #Расположения 
SELECT  [КодРасположения],
        Расположение,      
        [Parent],
        [L],
        [R],
        [Изменил],
        [Изменено], 
        4 BitMask
FROM    vwРасположения 
WHERE   Parent IS NULL                                  
        AND NOT EXISTS(SELECT * FROM #Расположения X WHERE vwРасположения.КодРасположения = X.КодРасположения) AND Закрыто=0

SELECT #Расположения.[КодРасположения] id,
       #Расположения.Расположение text,      
       #Расположения.[Parent] ParentId,
       #Расположения.[L],
       #Расположения.[R],
       #Расположения.[Изменил],
       #Расположения.[Изменено], 
       #Расположения.BitMask,
       #Расположения.R-#Расположения.L ЕстьДети,
       r.Офис Office,
       r.РабочееМесто WorkPlace,
       ISNULL(x.ЕстьСотрудники,0) ЕстьСотрудники, 
       ISNULL(rr.ЕстьОборудование,0) ЕстьОборудование
FROM #Расположения
INNER JOIN vwРасположения r ON #Расположения.КодРасположения = r.КодРасположения
LEFT JOIN 
        (SELECT COUNT(vwРеальноеРасположениеОборудования.КодРасположения) ЕстьОборудование, vwРеальноеРасположениеОборудования.КодРасположения
		FROM    vwРеальноеРасположениеОборудования
		GROUP BY КодРасположения) rr ON #Расположения.КодРасположения = rr.КодРасположения
LEFT JOIN 
        (SELECT COUNT(РабочиеМеста.КодРасположения) ЕстьСотрудники,РабочиеМеста.КодРасположения
		FROM    РабочиеМеста INNER JOIN 
                Сотрудники ON РабочиеМеста.КодСотрудника = Сотрудники.КодСотрудника
		WHERE   Сотрудники.Состояние = 0
		GROUP BY КодРасположения) x ON #Расположения.КодРасположения = x.КодРасположения
ORDER BY {0}
DROP TABLE #Расположения
";

        /// <summary>
        ///     Строка запроса: Фильтрация расположений
        /// </summary>
        public static string SELECT_РасположенияДанныеДляДерева_Фильтр = @"
DECLARE @МаксимальноеКоличествоНайденных int = 100
SET NOCOUNT ON
               
IF OBJECT_ID('tempdb.#Расположения') IS NOT NULL DROP TABLE #Расположения
--DECLARE @КоличествоНайденных int
CREATE TABLE #Расположения(
        КодЗаписи int IDENTITY(1,1),
        [КодРасположения] [int],
        Расположение [varchar](300),       
        [Parent] [int],
        [L] [int],
        [R] [int],
        [Изменил] [int],
        [Изменено] [datetime],  
        BitMask tinyint
)

INSERT #Расположения
SELECT  [КодРасположения],
        Расположение,      
        [Parent],
        [L],
        [R],
        [Изменил],
        [Изменено], 
        1 BitMask
FROM    vwРасположения 
WHERE   Расположение LIKE '{1}' AND Закрыто=0
ORDER BY L
 
SET @КоличествоНайденных = @@ROWCOUNT
DELETE #Расположения WHERE КодЗаписи > @МаксимальноеКоличествоНайденных
 
UPDATE  Parent
SET     BitMask = BitMask ^ 2
FROM    #Расположения Parent
WHERE   EXISTS(SELECT * FROM #Расположения Child WHERE Parent.L < Child.L AND Parent.R > Child.R)
 
INSERT  #Расположения
SELECT  [КодРасположения],
        Расположение,      
        [Parent],
        [L],
        [R],
        [Изменил],
        [Изменено], 
        2 BitMask
FROM    vwРасположения Parent 
WHERE   EXISTS( SELECT * FROM #Расположения Child 
                WHERE Parent.L <= Child.L AND Parent.R>=Child.R)                                        
        AND NOT EXISTS(SELECT * FROM #Расположения X WHERE Parent.КодРасположения = X.КодРасположения) AND Закрыто=0
 
UPDATE  #Расположения
SET     BitMask = BitMask ^ 4
WHERE   Parent IS NULL
 
INSERT #Расположения 
SELECT  [КодРасположения],
        Расположение,      
        [Parent],
        [L],
        [R],
        [Изменил],
        [Изменено], 
        4 BitMask
FROM    vwРасположения 
WHERE   Parent IS NULL                                  
        AND NOT EXISTS(SELECT * FROM #Расположения X WHERE vwРасположения.КодРасположения = X.КодРасположения) AND Закрыто=0
        
SELECT #Расположения.[КодРасположения] id,
       #Расположения.Расположение text,      
       #Расположения.[Parent] ParentId,
       #Расположения.[L],
       #Расположения.[R],
       #Расположения.[Изменил],
       #Расположения.[Изменено], 
       #Расположения.BitMask,
       #Расположения.R-#Расположения.L ЕстьДети,
       r.Офис Office,
       r.РабочееМесто WorkPlace,
       ISNULL(x.ЕстьСотрудники,0) ЕстьСотрудники, 
       ISNULL(rr.ЕстьОборудование,0) ЕстьОборудование
FROM #Расположения
INNER JOIN vwРасположения r ON #Расположения.КодРасположения = r.КодРасположения
LEFT JOIN 
        (SELECT COUNT(vwРеальноеРасположениеОборудования.КодРасположения) ЕстьОборудование, vwРеальноеРасположениеОборудования.КодРасположения
		FROM    vwРеальноеРасположениеОборудования
		GROUP BY КодРасположения) rr ON #Расположения.КодРасположения = rr.КодРасположения
LEFT JOIN 
        (SELECT COUNT(РабочиеМеста.КодРасположения) ЕстьСотрудники,РабочиеМеста.КодРасположения
		FROM    РабочиеМеста INNER JOIN 
                Сотрудники ON РабочиеМеста.КодСотрудника = Сотрудники.КодСотрудника
		WHERE   Сотрудники.Состояние = 0
		GROUP BY КодРасположения) x ON #Расположения.КодРасположения = x.КодРасположения
ORDER BY {0}
DROP TABLE #Расположения
";

        /// <summary>
        ///     Строка запроса: Добавить место работы сотруднику
        /// </summary>
        public const string INSERT_РабочееМестоСотруднику = @"
IF NOT EXISTS(SELECT * FROM vwРасположения WHERE КодРасположения = @КодРасположения) 
BEGIN RAISERROR  (N'Указанное расположение в базе данных отсутствует - %s.', 12, 1, @КодРасположения) RETURN END
        
IF NOT EXISTS(SELECT * FROM РабочиеМеста WHERE КодСотрудника = @КодСотрудника AND КодРасположения = @КодРасположения)
INSERT INTO РабочиеМеста (КодСотрудника, КодРасположения) VALUES (@КодСотрудника, @КодРасположения)";

        /// <summary>
        ///     Обновление наименования расположения
        /// </summary>
        public const string UPDATE_Расположения = @"
UPDATE Инвентаризация.dbo.Расположения SET Расположение=@Расположение WHERE КодРасположения=@КодРасположения
";

        /// <summary>
        ///     Обновить РабочееМесто у расположения
        /// </summary>
        public const string UPDATE_Расположения_РабочееМесто = @"
UPDATE Инвентаризация.dbo.Расположения SET РабочееМесто=@РабочееМесто WHERE КодРасположения=@КодРасположения
";

        /// <summary>
        ///     Обновить Офис у расположения
        /// </summary>
        public const string UPDATE_Расположения_Офис = @"
UPDATE Инвентаризация.dbo.Расположения SET Офис=@Офис WHERE КодРасположения=@КодРасположения
";

        /// <summary>
        ///     Строка запроса: розетка по id
        /// </summary>
        public const string SELECT_РозеткаПоID = @"
SELECT КодРасположения, Розетка, Работает, Примечание FROM Инвентаризация.dbo.vwРозетки WHERE КодРозетки=@id
";

        /// <summary>
        ///     Добавить розетку
        /// </summary>
        public const string INSERT_Розетки = @"
INSERT INTO Инвентаризация.dbo.vwРозетки (КодРасположения, Розетка, Работает, Примечание) VALUES (@КодРасположения, @Розетка, @Работает, @Примечание)
";

        /// <summary>
        ///     Обновить розетку
        /// </summary>
        public const string Update_Розетки = @"
UPDATE Инвентаризация.dbo.vwРозетки SET Розетка = @Розетка, Работает = @Работает, Примечание = @Примечание WHERE КодРозетки = @КодРозетки
";


        /// <summary>
        ///     Удалить розетку
        /// </summary>
        public const string DELETE_ID_Розетки = @"
DELETE FROM Инвентаризация.dbo.vwРозетки where КодРозетки = @id
";

        /// <summary>
        ///     Добавить расположение и получить его идентификатор
        /// </summary>
        public const string INSERT_Расположения = @"
INSERT Инвентаризация.dbo.Расположения (Расположение, РабочееМесто, Parent) VALUES (@Расположение, @РабочееМесто, @Parent)
SELECT @@IDENTITY";

        #endregion

        #region Должности

        /// <summary>
        ///     Строка запроса: Получение Должности
        /// </summary>
        public const string SELECT_Должности = @"
SELECT  DISTINCT {0} T0.Должность
FROM    vwДолжности T0 INNER JOIN 
        ЛицаЗаказчики T1 ON T0.КодЛица = T1.КодЛица
WHERE   T0.Должность <> ''";

        /// <summary>
        ///     Строка запроса: Получение Подразделения
        /// </summary>
        public const string SELECT_Подразделения = @"
SELECT  DISTINCT {0} T0.Подразделение
FROM    vwДолжности T0 INNER JOIN 
        ЛицаЗаказчики T1 ON T0.КодЛица = T1.КодЛица
WHERE   T0.Подразделение <> ''";

        /// <summary>
        ///     Строка запроса: Получение Подразделений Лица
        /// </summary>
        public const string SELECT_ПодразделенияЛиц =
            @"SELECT {0} КодПодразделенияЛица, Подразделение FROM vwПодразделенияЛиц T0";


        /// <summary>
        ///     Строка запроса: Поиск Должность по коду
        /// </summary>
        public const string SELECT_ID_Должность = @"
SELECT КодДолжности, Должность, Подразделение, КодЛица, КодСотрудника, Совместитель, Оклад, ТарифнаяСтавка, Parent, L, R, Изменил, Изменено
FROM vwДолжности (nolock) WHERE КодДолжности = @id";

        /// <summary>
        ///     Строка запроса: Получить должности сотрудника
        /// </summary>
        public static string SELECT_ДолжностиСотрудника = @"
SELECT  vwДолжности.КодДолжности, vwДолжности.Должность, vwДолжности.КодСотрудника, vwДолжности.КодЛица, vwДолжности.Совместитель,
        ISNULL(NULLIF (ЛицаЗаказчики.КраткоеНазваниеРус, ''), ЛицаЗаказчики.КраткоеНазваниеЛат) Организация, Подразделение,
        vwДолжности.КодЛица КодЛицаКомпанииСотрудника, vwДолжности.Должность ДолжностьСотрудника
FROM    vwДолжности LEFT JOIN 
        ЛицаЗаказчики ON vwДолжности.КодЛица = ЛицаЗаказчики.КодЛица
WHERE   vwДолжности.КодСотрудника = @КодСотрудника AND (@Совместитель = -1 OR vwДолжности.Совместитель = @Совместитель)
ORDER BY vwДолжности.Совместитель";

        /// <summary>
        ///     Строка запроса: Сотруднику по должности положена Sim-карта
        /// </summary>
        public const string SELECT_СотрудникуПоДолжностиПоложенаSIM = @"
SELECT  НеобходимаSIMКарта, ПодключитьGPRSПакет, Должность
FROM    vwДолжности 
WHERE   КодСотрудника = @КодСотрудника AND НеобходимаSIMКарта <> 0
";

        #endregion

        #region Отчёты по складам

        /// <summary>
        ///     Строка запроса: ТипОтчётаПоСкладам
        /// </summary>
        public static string SELECT_ТипОтчётаПоСкладам = @"
SELECT {0} КодТипаОтчётаПоСкладам, ТипОтчётаПоСкладам FROM ТипыОтчётовПоСкладам ТО WHERE EXISTS(SELECT * FROM Инвентаризация.dbo.fn_ТекущиеРоли() X
			WHERE X.КодРоли IN (ТО.КодРоли1, ТО.КодРоли2))";

/*
         public static string SELECT_ОтчётПоСкладам = @"
SELECT TOP 1 КодТипаОтчётаПоСкладам FROM vwОтчётыПоСкладам WHERE КодСклада=@КодСклада AND КодТипаОтчётаПоСкладам=@КодТипаОтчётаПоСкладам ORDER BY (SELECT NULL)";

         public static string SELECT_КодТипаОтчётаПоСкладам = @"
SELECT TOP 1 КодТипаОтчётаПоСкладам FROM vwОтчётыПоСкладам WHERE КодСклада=@КодСклада ORDER BY (SELECT NULL)";
*/
        /// <summary>
        ///     Строка запроса: КодыТиповОтчётовПоСкладам
        /// </summary>
        public static string SELECT_КодыТиповОтчётовПоСкладам = @"
SELECT vwОтчётыПоСкладам.КодТипаОтчётаПоСкладам, ТипОтчётаПоСкладам FROM vwОтчётыПоСкладам INNER JOIN ТипыОтчётовПоСкладам ON vwОтчётыПоСкладам.КодТипаОтчётаПоСкладам=ТипыОтчётовПоСкладам.КодТипаОтчётаПоСкладам WHERE КодСклада=@КодСклада ORDER BY ТипыОтчётовПоСкладам.ТипОтчётаПоСкладам";

        /// <summary>
        ///     Строка запроса: ОтчётыПоСкладам
        /// </summary>
        public static string SELECT_ОтчётыПоСкладам = @"
SELECT
    Типы.ТипСклада,
    Склады.IBAN,
    Отчёты.Склад,
    Отчёты.Порядок,
    Отчёты.КодТипаОтчётаПоСкладам,
    Отчёты.Хранитель,
    Отчёты.Распорядитель,
    Отчёты.Ресурс,
    Отчёты.КодСклада,
    Отчёты.КодХранителя,
    Отчёты.КодРаспорядителя,
    Отчёты.От,
    DATEADD(day,-1, Отчёты.До) AS По
FROM vwОтчётыПоСкладам AS Отчёты
INNER JOIN vwСклады AS Склады ON Отчёты.КодСклада = Склады.КодСклада
INNER JOIN ТипыСкладов AS Типы ON Склады.КодТипаСклада = Типы.КодТипаСклада
WHERE КодТипаОтчётаПоСкладам=@КодТипаОтчётаПоСкладам ORDER BY Отчёты.Порядок";

        /// <summary>
        ///     Строка запроса: СкладыВОтчете
        /// </summary>
        public static string SELECT_СкладыВОтчете = @"
SELECT
КодСклада, Порядок
FROM vwОтчётыПоСкладам
WHERE КодТипаОтчётаПоСкладам=@КодТипаОтчётаПоСкладам ORDER BY Порядок";

        /// <summary>
        ///     Строка запроса: Добавление отчета по складам
        /// </summary>
        public static string INSERT_СкладОтчётыПоСкладам = @"
INSERT vwОтчётыПоСкладам(КодСклада, КодТипаОтчётаПоСкладам, Порядок)
SELECT T0.КодСклада, T1.КодОтчёта, T1.СледПорядок FROM (SELECT @КодСклада КодСклада) AS T0
CROSS JOIN(
SELECT value AS КодОтчёта, ISNULL(T12.МаксПорядок,0)+1 AS СледПорядок FROM Инвентаризация.dbo.fn_SplitInts(@КодыТиповОтчётаПоСкладам) AS T11 LEFT JOIN (SELECT КодТипаОтчётаПоСкладам, MAX(Порядок) AS МаксПорядок FROM vwОтчётыПоСкладам GROUP BY КодТипаОтчётаПоСкладам) AS T12 ON T12.КодТипаОтчётаПоСкладам=T11.value
) AS T1
LEFT JOIN vwОтчётыПоСкладам AS T2 ON T2.КодСклада = T0.КодСклада AND КодТипаОтчётаПоСкладам=T1.КодОтчёта
WHERE КодТипаОтчётаПоСкладам is NULL";

        /// <summary>
        ///     Строка запроса: Обновление СкладОтчётыПоСкладам
        /// </summary>
        public const string UPDATE_СкладОтчётыПоСкладам = @"
UPDATE vwОтчётыПоСкладам SET Порядок=T1.Порядок
FROM (VALUES {0}) AS T1(Код, Порядок)
WHERE КодСклада = T1.Код AND КодТипаОтчётаПоСкладам = @КодТипаОтчётаПоСкладам";

        /// <summary>
        ///     Строка запроса: Удаление СкладыИзОтчётаПоСкладам
        /// </summary>
        public const string DELETE_СкладыИзОтчётаПоСкладам = @"
DELETE FROM vwОтчётыПоСкладам WHERE КодСклада IN(SELECT value FROM Инвентаризация.dbo.fn_SplitInts(@КодыСкладов)) AND КодТипаОтчётаПоСкладам=@КодТипаОтчётаПоСкладам";

        /// <summary>
        ///     Строка запроса: Удаление ВсеСкладыИзОтчётаПоСкладам
        /// </summary>
        public const string DELETE_ВсеСкладыИзОтчётаПоСкладам = @"
DELETE FROM vwОтчётыПоСкладам WHERE КодТипаОтчётаПоСкладам=@КодТипаОтчётаПоСкладам";

        /// <summary>
        ///     Строка запроса: Удаление СкладИзОтчётовПоСкладам
        /// </summary>
        public const string DELETE_СкладИзОтчётовПоСкладам = @"
DELETE FROM vwОтчётыПоСкладам WHERE КодСклада = @КодСклада AND КодТипаОтчётаПоСкладам NOT IN(SELECT value FROM Инвентаризация.dbo.fn_SplitInts(@КодТипаОтчётаПоСкладам))";

        /// <summary>
        ///     Строка запроса: ОтчётыПоСкладам
        /// </summary>
        public const string MERGE_СкладОтчётыПоСкладам = @"
;WITH СкладПланы AS
(
SELECT КодТипаОтчётаПоСкладам, КодСклада, Порядок
FROM vwОтчётыпоСкладам WHERE КодСклада=@КодСклада
)
MERGE INTO СкладПланы
USING 
(SELECT T1.КодОтчёта, T0.КодСклада, T1.СледПорядок FROM (SELECT @КодСклада КодСклада) AS T0
CROSS JOIN(
SELECT value AS КодОтчёта, ISNULL(T12.МаксПорядок,0)+1 AS СледПорядок FROM Инвентаризация.dbo.fn_SplitInts(@КодыТиповОтчётаПоСкладам) AS T11 LEFT JOIN (SELECT КодТипаОтчётаПоСкладам, MAX(Порядок) AS МаксПорядок FROM vwОтчётыПоСкладам GROUP BY КодТипаОтчётаПоСкладам) AS T12 ON T12.КодТипаОтчётаПоСкладам=T11.value
) AS T1) AS НовыеПланы(КодТипаОтчётаПоСкладам, КодСклада, Порядок)
ON НовыеПланы.КодТипаОтчётаПоСкладам = СкладПланы.КодТипаОтчётаПоСкладам
WHEN NOT MATCHED THEN INSERT VALUES(НовыеПланы.КодТипаОтчётаПоСкладам, НовыеПланы.КодСклада, НовыеПланы.Порядок)
WHEN NOT MATCHED BY SOURCE THEN DELETE;";

        /// <summary>
        ///     Строка запроса: ОтчётПоСкладам
        /// </summary>
        public const string MERGE_СкладыОтчётПоСкладам = @"
;WITH СкладыПлан AS
(
SELECT КодТипаОтчётаПоСкладам, КодСклада, Порядок
FROM dbo.vwОтчётыпоСкладам WHERE КодТипаОтчётаПоСкладам=@КодТипаОтчётаПоСкладам
),
НовыйПлан AS
(
SELECT КодТипаОтчётаПоСкладам,КодСклада,Порядок FROM (SELECT @КодТипаОтчётаПоСкладам) AS T1(КодТипаОтчётаПоСкладам) CROSS JOIN (VALUES {0}) AS T2(КодСклада, Порядок)
)
MERGE INTO СкладыПлан
USING НовыйПлан
ON НовыйПлан.КодСклада = СкладыПлан.КодСклада
WHEN NOT MATCHED THEN INSERT VALUES(@КодТипаОтчётаПоСкладам, НовыйПлан.КодСклада, НовыйПлан.Порядок)
WHEN MATCHED THEN UPDATE SET СкладыПлан.Порядок = НовыйПлан.Порядок
WHEN NOT MATCHED BY SOURCE THEN DELETE;";

        #endregion

        #region Территории и телефонные коды

        /// <summary>
        ///     Строка запроса: Получаем территории - страны
        /// </summary>
        public const string SELECT_Территории_Страны = @"
SELECT  КодТерритории, Территория, Caption, Аббревиатура, ТелКодСтраны
FROM    vwТерритории (nolock)
WHERE   КодТТерритории = 2";

        /// <summary>
        ///     Строка запроса: Получаем территорию - страну по ID
        /// </summary>
        public const string SELECT_ID_Территории_Страна = @"
SELECT  КодТерритории, Территория, Caption, Аббревиатура, ТелКодСтраны
FROM    vwТерритории (nolock) 
WHERE   КодТерритории = @id AND КодТТерритории = 2";

        /// <summary>
        ///     Строка запроса: Получение информации о территориях для дерева
        /// </summary>
        public const string SELECT_ТерриторииДанныеДляДерева = @"
SELECT  r.КодТерритории Id, r.R-L ЕстьДети, r.Территория Text, ISNULL(r.Parent,0) ParentId, '' Фильтр
FROM vwТерритории r
        WHERE((@Потомки = 1 AND ((@Код = 0 AND r.Parent IS NULL) OR(@Код<> 0 AND r.Parent = @Код)))
        OR(@Потомки = 0 AND r.КодТерритории = @Код)) 
        AND r.R-r.L > 1
ORDER BY r.{0}";

        /// <summary>
        ///     Строка запроса: Получение информации о территориях для дерева
        /// </summary>
        public const string SELECT_ТерриторииДанныеДляДерева_State = @"
SET NOCOUNT ON

IF OBJECT_ID('tempdb.#Территории') IS NOT NULL DROP TABLE #Территории
CREATE TABLE #Территории(
        КодЗаписи int IDENTITY(1,1),
        [КодТерритории] [int],
        Территория [varchar](300),       
        [Parent] [int],
        [L] [int],
        [R] [int],
        [Изменил] [int],
        [Изменено] [datetime],  
        BitMask tinyint
)

INSERT #Территории
SELECT	Parent.[КодТерритории],
        Parent.Территория,      
        Parent.[Parent],
        Parent.[L],
        Parent.[R],
        Parent.[Изменил],
        Parent.[Изменено], 
        1 BitMask
FROM	vwТерритории Parent 
WHERE EXISTS(SELECT * FROM vwТерритории Child 
					WHERE	Child.КодТерритории IN ({1})
						AND Parent.L <=	Child.L AND Parent.R>=Child.R)
      AND NOT EXISTS(SELECT * FROM #Территории X WHERE Parent.КодТерритории = X.КодТерритории)
ORDER BY Parent.L

INSERT #Территории
SELECT	Child.[КодТерритории],
        Child.Территория,      
        Child.[Parent],
        Child.[L],
        Child.[R],
        Child.[Изменил],
        Child.[Изменено], 
        2 BitMask
FROM	vwТерритории Parent
LEFT JOIN vwТерритории Child ON Child.Parent = Parent.КодТерритории
WHERE Parent.КодТерритории IN ({1}) AND NOT EXISTS(SELECT * FROM #Территории X WHERE Child.КодТерритории = X.КодТерритории)
ORDER BY Parent.L

INSERT #Территории 
SELECT  [КодТерритории],
        Территория,      
        [Parent],
        [L],
        [R],
        [Изменил],
        [Изменено], 
        4 BitMask
FROM    vwТерритории 
WHERE   Parent IS NULL                                  
        AND NOT EXISTS(SELECT * FROM #Территории X WHERE vwТерритории.КодТерритории = X.КодТерритории)

SELECT t.[КодТерритории] id,
       t.Территория text,      
       t.[Parent] ParentId,
       t.[L],
       t.[R],
       t.[Изменил],
       t.[Изменено], 
       t.BitMask,
       t.R-t.L ЕстьДети
FROM #Территории t
INNER JOIN vwТерритории r ON t.КодТерритории = r.КодТерритории
ORDER BY {0}
DROP TABLE #Территории
";

        /// <summary>
        ///     Строка запроса: Получение информации о территориях для дерева, учитывая фильтр
        /// </summary>
        public static string SELECT_ТерриторииДанныеДляДереваФильтр = @"
DECLARE @МаксимальноеКоличествоНайденных int = 100
SET NOCOUNT ON
               
IF OBJECT_ID('tempdb.#Территории') IS NOT NULL DROP TABLE #Территории
--DECLARE @КоличествоНайденных int
CREATE TABLE #Территории(
        КодЗаписи int IDENTITY(1,1),
        [КодТерритории] [int],
        Территория [varchar](100),       
        [Parent] [int],
        [L] [int],
        [R] [int],
        [Изменил] [int],
        [Изменено] [datetime],  
        BitMask tinyint
)

INSERT #Территории
SELECT  [КодТерритории],
        Территория,      
        [Parent],
        [L],
        [R],
        [Изменил],
        [Изменено], 
        1 BitMask
FROM    vwТерритории
WHERE   Территория LIKE '{1}' AND Закрыто=0
ORDER BY L
 
SET @КоличествоНайденных = @@ROWCOUNT
DELETE #Территории WHERE КодЗаписи > @МаксимальноеКоличествоНайденных
 
UPDATE  Parent
SET     BitMask = BitMask ^ 2
FROM    #Территории Parent
WHERE   EXISTS(SELECT * FROM #Территории Child WHERE Parent.L < Child.L AND Parent.R > Child.R)
 
INSERT  #Территории
SELECT  [КодТерритории],
        Территория,      
        [Parent],
        [L],
        [R],
        [Изменил],
        [Изменено], 
        2 BitMask
FROM    vwТерритории Parent 
WHERE   EXISTS( SELECT * FROM #Территории Child 
                WHERE Parent.L <= Child.L AND Parent.R>=Child.R)                                        
        AND NOT EXISTS(SELECT * FROM #Территории X WHERE Parent.КодТерритории = X.КодТерритории)
 
UPDATE  #Территории
SET     BitMask = BitMask ^ 4
WHERE   Parent IS NULL
 
INSERT #Территории 
SELECT  [КодТерритории],
        Территория,      
        [Parent],
        [L],
        [R],
        [Изменил],
        [Изменено], 
        4 BitMask
FROM    vwТерритории 
WHERE   Parent IS NULL                                  
        AND NOT EXISTS(SELECT * FROM #Территории X WHERE vwТерритории.КодТерритории = X.КодТерритории)
        
SELECT #Территории.[КодТерритории] id,
       #Территории.Территория text,      
       #Территории.[Parent] ParentId,
       #Территории.[L],
       #Территории.[R],
       #Территории.[Изменил],
       #Территории.[Изменено], 
       #Территории.BitMask,
       #Территории.R-#Территории.L ЕстьДети
FROM #Территории
INNER JOIN vwТерритории r ON #Территории.КодТерритории = r.КодТерритории
ORDER BY {0}
DROP TABLE #Территории
";
        /// <summary>
        ///     Строка запроса: Получение информации подчиненных территориях
        /// </summary>
        public static string SELECT_ТерриторииПодчиненные = @"
SELECT  КодТерритории, Территория, Caption, Территория1, КодОКСМ, Направление, ТелКодСтраны, L, R
FROM vwТерритории
WHERE Parent = @Код";


        /// <summary>
        ///     Строка запроса: Получаем телефонный код города по ID территории
        /// </summary>
        public const string SELECT_ID_Территории_ТелефонныйКодГорода = @"
SELECT  TOP 1 SUBSTRING(ТелефонныйКод, ISNULL(LEN(ТелКодСтраны),0)+1, ДлинаКодаОбласти) ТелКодВСтране
FROM    ТелефонныеКоды INNER JOIN 
        Территории ON ТелефонныеКоды.КодТерритории = Территории.КодТерритории
WHERE   Территории.КодТерритории = @id AND КодТТерритории = 4";

        /// <summary>
        ///     Строка запроса: Получаем информацию о телефонах по коду территории
        /// </summary>
        public const string SELECT_ID_Территории_ТелефонныеКоды = @"
SELECT  ТелефонныеКоды.КодТерритории, Направление, Территории.ТелКодСтраны,
        SUBSTRING(ТелефонныйКод, ISNULL(LEN(ТелКодСтраны),0)+1, ДлинаКодаОбласти) ТелКодВСтране,  
        SUBSTRING(ТелефонныйКод, ISNULL(LEN(ТелКодСтраны),0)+ДлинаКодаОбласти+1, LEN(ТелефонныйКод)) as Телефон
FROM    ТелефонныеКоды INNER JOIN 
        Территории ON ТелефонныеКоды.КодТерритории = Территории.КодТерритории
WHERE   Территории.КодТерритории = @id";

        /// <summary>
        ///     Строка запроса: Получаем информацию о телефоне по номеру телефона
        /// </summary>
        public const string SELECT_ТелефонныеКоды = @"
SELECT  TOP 1 ТелефонныеКоды.КодТерритории, Направление, ТелКодСтраны,
        SUBSTRING(ТелефонныйКод, ISNULL(LEN(ТелКодСтраны),0)+1, LEN(ТелефонныйКод)) ТелКодВСтране,
        SUBSTRING(@phone, ISNULL(LEN(ТелКодСтраны),0)+ДлинаКодаОбласти+1, LEN(@phone)) as Телефон
FROM    ТелефонныеКоды INNER JOIN 
        Территории ON ТелефонныеКоды.КодТерритории = Территории.КодТерритории
WHERE   @phone LIKE ТелефонныеКоды.ТелефонныйКод + '%'
ORDER BY LEN(ТелефонныеКоды.ТелефонныйКод) DESC";

        /// <summary>
        ///     Строка запроса: Получить направление, ТелКодСтраны, ТелКодВСтране, ДлинаКодаОбласти и название территории из
        ///     телефонного номера
        /// </summary>
        public const string SELECT_ЧастиТелефонногоНомера = @"
SELECT  TOP 1 @Направление = Направление,
        @ТелКодСтраны = ТелКодСтраны,
        @ТелКодВСтране = SUBSTRING(ТелефонныйКод, ISNULL(LEN(ТелКодСтраны),0)+1, LEN(ТелефонныйКод)),
        @ДлинаКодаОбласти = ТелефонныеКоды.ДлинаКодаОбласти,
        @Территория = Территория
FROM ТелефонныеКоды INNER JOIN Территории ON ТелефонныеКоды.КодТерритории = Территории.КодТерритории
WHERE (@Телефон LIKE ТелефонныйКод + '%')
ORDER BY LEN(ТелефонныйКод) DESC";

        #endregion

        #region Тарификация мобильной связи

        /// <summary>
        ///     Строка запроса: Получение договора сотовой связи по ID
        /// </summary>
        public const string SELECT_ID_ДоговорСотовойСвязи =
            @"SELECT КодДокумента КодДоговора, Д.Исполнитель + ' - ' + Д.Заказчик + ISNULL(' №' + NULLIF(Д.НомерДокумента,''),'') Договор FROM Инвентаризация.dbo.ДоговораСвязи Д (nolock) WHERE Сотовая = 1 AND КодДокумента = @id";

        /// <summary>
        ///     Строка запроса: Договора сотовой связи, по которым существует тарификация мобильной связи
        /// </summary>
        public const string SELECT_ДоговораПоКоторымБылаТарификация = @"
SELECT КодДокумента КодДоговора, T0.Исполнитель + ' - ' + T0.Заказчик + ISNULL(' №' + NULLIF(T0.НомерДокумента,''),'') Договор 
FROM Инвентаризация.dbo.ДоговораСвязи T0 (nolock) 
WHERE EXISTS(SELECT * FROM Тарификация.dbo.vwТарификацияСотовыхИтоги Т (nolock) 
            WHERE   Т.КодДоговора = T0.КодДоговора
                    AND ((@OpenMonth = 1 AND Год = 0 AND Месяц = 0)
			            OR (@OpenMonth = 0 AND ((@Год IS NULL OR Год = @Год) AND (@Месяц IS NULL OR Месяц = @Месяц))))
) 
";

        #endregion

        #region Роли

        /// <summary>
        ///     Строка запроса: Получить роли сотрудника
        /// </summary>
        public static string SELECT_РолиСотрудника = @" 
SELECT  РолиСотрудников.КодРоли, 
	    РолиСотрудников.КодСотрудника, 
	    РолиСотрудников.КодЛица,
	    CASE WHEN ЛицаЗаказчики.КодЛица IS NULL THEN '' ELSE 
	        ISNULL(NULLIF(CASE WHEN ЛицаЗаказчики.КраткоеНазваниеРус = '' THEN ЛицаЗаказчики.КраткоеНазваниеЛат ELSE ЛицаЗаказчики.КраткоеНазваниеРус END,''), ЛицаЗаказчики.Кличка) 
	    END НазваниеЛица,
	    vwРоли.Роль, 
	    vwРоли.Описание 
FROM    РолиСотрудников LEFT JOIN 
        vwРоли ON РолиСотрудников.КодРоли = vwРоли.КодРоли LEFT JOIN 
        ЛицаЗаказчики ON РолиСотрудников.КодЛица = ЛицаЗаказчики.КодЛица
WHERE   РолиСотрудников.КодСотрудника = @КодСотрудника";

        /// <summary>
        ///     Строка запроса: Получить роли текущего сотрудника, учитывая замещения
        /// </summary>
        public static string SELECT_РолиТекущегоСотрудника = @" 
SELECT  РолиСотрудника.КодРоли, 
	    @КодСотрудника AS КодСотрудника, 
	    РолиСотрудника.КодЛица,
	    CASE WHEN ЛицаЗаказчики.КодЛица IS NULL THEN '' ELSE 
	        ISNULL(NULLIF(CASE WHEN ЛицаЗаказчики.КраткоеНазваниеРус = '' THEN ЛицаЗаказчики.КраткоеНазваниеЛат ELSE ЛицаЗаказчики.КраткоеНазваниеРус END,''), ЛицаЗаказчики.Кличка) 
	    END НазваниеЛица,
	    vwРоли.Роль, 
	    vwРоли.Описание 
FROM    fn_ТекущиеРоли() РолиСотрудника LEFT JOIN 
        vwРоли ON РолиСотрудника.КодРоли = vwРоли.КодРоли LEFT JOIN 
        ЛицаЗаказчики ON РолиСотрудника.КодЛица = ЛицаЗаказчики.КодЛица";

        /// <summary>
        ///     Строка запроса: список ролей
        /// </summary>
        public static string SELECT_Роли = @" 
SELECT * FROM vwРоли";

        /// <summary>
        ///     Строка запроса: роль по id
        /// </summary>
        public static string SELECT_ID_Роль = @" 
SELECT * FROM vwРоли WHERE КодРоли = @id";

        /// <summary>
        ///     Строка запроса: Получить роль сорудника
        /// </summary>
        public static string SELECT_РолиСотрудника_КодСотрудника_КодРоли_КодЛица = @" 
SELECT * FROM РолиСотрудников WHERE КодСотрудника = @КодСотрудника AND КодРоли=@КодРоли AND КодЛица=@КодЛица";

        /// <summary>
        ///     Запрос для поиска ролей в приложении Роли сотрудников
        /// </summary>
        public static string SELECT_РолиПоиск = @"
DECLARE @Язык char(2)
SELECT @Язык = Язык FROM Сотрудники WHERE SID=SUSER_SID()

SELECT	РолиСотрудников.КодСотрудника, 
	    Сотрудники.ФИО,          
	    Сотрудник =     CASE    WHEN @Язык = 'ru' THEN Сотрудники.Сотрудник + CASE WHEN Сотрудники.Дополнение <> '' THEN ' (' + Сотрудники.Дополнение + ')' ELSE '' END
				            ELSE Сотрудники.Employee + CASE WHEN Сотрудники.Addition <> '' THEN ' (' + Сотрудники.Addition + ')' ELSE '' END
			        END,
	    vwРоли.Роль,
	    РолиСотрудников.КодЛица, 
	    ISNULL(Лица.Кличка,CASE WHEN РолиСотрудников.КодЛица=0 THEN '<Все организации>' ELSE '#'+CONVERT(varchar,РолиСотрудников.КодЛица) END) Организация, 
	    РолиСотрудников.КодРоли
FROM    РолиСотрудников INNER JOIN
	    Сотрудники ON РолиСотрудников.КодСотрудника = Сотрудники.КодСотрудника INNER JOIN
	    vwРоли ON РолиСотрудников.КодРоли = vwРоли.КодРоли LEFT JOIN
	    {0}.Справочники.dbo.vwЛица Лица ON РолиСотрудников.КодЛица = Лица.КодЛица
WHERE	(@КодРоли = -1 OR РолиСотрудников.КодРоли = @КодРоли) AND 
	    (@КодЛица = -1 OR РолиСотрудников.КодЛица = @КодЛица OR (@КодЛица > 0 AND РолиСотрудников.КодЛица=0))
";

        /// <summary>
        ///     Строка запроса: Добавление роли сотруднику
        /// </summary>
        public static string INSERT_РолиСотрудников = @"
IF NOT EXISTS (SELECT * FROM РолиСотрудников WHERE КодРоли = @КодРоли AND КодСотрудника = @КодСотрудника AND КодЛица = @КодЛица)
    INSERT РолиСотрудников(КодРоли, КодСотрудника, КодЛица) VALUES(@КодРоли, @КодСотрудника, @КодЛица)
";

        /// <summary>
        ///     Строка запроса: Добавление роли сотруднику
        /// </summary>
        public static string INSERT_РолиСотрудников_БизнесПроект = @"
DECLARE @Лица TABLE(КодЛица int)

INSERT  @Лица
SELECT  КодЛица FROM {0}.Справочники.dbo.vwЛица WHERE КодБизнесПроекта = @КодБизнесПроекта

INSERT  РолиСотрудников(КодРоли, КодСотрудника, КодЛица)
SELECT  @КодРоли, @КодСотрудника, КодЛица FROM @Лица X
WHERE   NOT EXISTS(SELECT * FROM РолиСотрудников Y WHERE Y.КодРоли = @КодРоли AND Y.КодСотрудника = @КодСотрудника AND (Y.КодЛица = X.КодЛица OR Y.КодЛица = 0))
";

        /// <summary>
        ///     Удалить роль сотрудника @КодСотрудника, @КодРоли, @КодЛица
        /// </summary>
        public static string DELETE_РолиСотрудников = @"
DELETE РолиСотрудников WHERE КодСотрудника = @КодСотрудника AND КодРоли=@КодРоли AND КодЛица=@КодЛица";

        #endregion

        #region Оборудование

        /// <summary>
        ///     Строка запроса: Получить ответственного за оборудование
        /// </summary>
        public const string SELECT_ID_ОборудованиеСотрудников = @"
DECLARE @Язык char(2)
SELECT @Язык = Язык FROM Сотрудники WHERE SID=SUSER_SID()

SELECT  Сотрудники.КодСотрудника, CASE WHEN @Язык <> 'ru' THEN Сотрудники.Employee ELSE Сотрудники.Сотрудник END Сотрудник
FROM    ОборудованиеСотрудников INNER JOIN 
        Сотрудники ON ОборудованиеСотрудников.КодСотрудника = Сотрудники.КодСотрудника
WHERE   ОборудованиеСотрудников.КодОборудования = @id AND ОборудованиеСотрудников.До IS NULL
ORDER BY Сотрудник
";

        /// <summary>
        ///     Строка запроса: Получить оборудование по идентификатору
        /// </summary>
        public const string SELECT_ID_Оборудование = @"
SELECT * FROM vwОборудование WHERE КодОборудования = @id";

        /// <summary>
        ///     Строка запроса: Получить модель оборудования по идентификатору
        /// </summary>
        public const string SELECT_ID_МодельОборудования = @"
SELECT * FROM МоделиОборудоваия WHERE КодМоделиОборудования = @id";

        /// <summary>
        ///     Строка запроса: Получить тип оборудования по идентификатору
        /// </summary>
        public const string SELECT_ID_ТипыОборудования = @"
SELECT * FROM ТипыОборудоваия WHERE КодТипаОборудования = @id";

        /// <summary>
        ///     Строка запроса: Получить информацию об оборудовании по коду сотрудника
        /// </summary>
        public const string SELECT_ID_ОборудованиеСотрудникаНаДругихРабочихМестах = @"
SELECT	КодОборудования, КодТипаОборудования, ТипОборудования, КодМоделиОборудования, МодельОборудования, КодРасположения, РасположениеPath, КодСотрудника, ФИО Сотрудник --, Изменил, Изменено 
FROM	vwОборудованиеСписок 
WHERE	КодСотрудника = @id AND (@IT=0 OR @IT = 1 AND КодТипаОборудования < 100)
	AND Списано IS NULL
	AND КодРасположения NOT IN(55,86) --не на руках и не в ремонте
	AND NOT EXISTS(SELECT * FROM РабочиеМеста 
	               WHERE	РабочиеМеста.КодСотрудника= vwОборудованиеСписок.КодСотрудника 
				AND РабочиеМеста.КодРасположения=vwОборудованиеСписок.КодРасположения
				)
ORDER BY ТипОборудования, МодельОборудования";

        /// <summary>
        ///     Строка запроса: Получить информацию об оборудовании по коду расположения
        /// </summary>
        public const string SELECT_ID_ОборудованиеНаРасположении = @"
SELECT	КодОборудования, КодТипаОборудования, ТипОборудования, КодМоделиОборудования, МодельОборудования, КодРасположения, РасположениеPath, КодСотрудника, ФИО Сотрудник --, Изменил, Изменено 
FROM	vwОборудованиеСписок 
WHERE	КодРасположения = @id AND (@IT=0 OR @IT = 1 AND КодТипаОборудования < 100)
	    AND Списано IS NULL	
ORDER BY ТипОборудования, МодельОборудования";

        #endregion

        #region Ивентаризщация

        /// <summary>
        ///     Строка запроса: Получение списка запросов указанного типа
        /// </summary>
        public const string SELECT_ЗапросыПоТипу = @"
SELECT * FROM Запросы WHERE ТипЗапроса=@КодТипаЗапроса ORDER BY КодЗапроса
";

        /// <summary>
        ///     Строка запроса: Получение информации о ближайшем принтере к расположению
        /// </summary>
        public const string SELECT_БлижайшийПринтер = @"
SELECT dbo.fn_БлижайшийПринтер (@L, @R) 
";

        /// <summary>
        ///     Строка запроса: Сорудники в указанном расположении
        /// </summary>
        public const string SELECT_СотрудникиВРасположении = @"
SELECT	РабочиеМеста.КодСотрудника, Сотрудники.Сотрудник 
FROM	РабочиеМеста INNER JOIN 
	    Сотрудники ON РабочиеМеста.КодСотрудника = Сотрудники.КодСотрудника
WHERE	РабочиеМеста.КодРасположения = @КодРасположения 
ORDER BY Сотрудники.Сотрудник
";

        /// <summary>
        ///     Строка запроса: РасположенияЛиц
        /// </summary>
        public const string SELECT_РасположенияЛицВРасположении = @"
        DECLARE @HAS_PERMS int = 0
        SELECT @HAS_PERMS = HAS_PERMS_BY_NAME('РасположенияЛиц', 'OBJECT', 'select') 
        IF (@HAS_PERMS = 1)
        SELECT ЛицаЗаказчики.Кличка Компания
            FROM    РасположенияЛиц INNER JOIN
            ЛицаЗаказчики ON РасположенияЛиц.КодЛица = ЛицаЗаказчики.КодЛица
            WHERE РасположенияЛиц.КодРасположения = @КодРасположения
            ELSE
                SELECT '' Компания WHERE 1=0
";

        /// <summary>
        ///     Строка запроса: Работающие сорудники в указанном расположении
        /// </summary>
        public const string SELECT_РаботающиеСотрудникиВРасположении = @" 
SELECT РабочиеМеста.КодСотрудника, Сотрудники.Сотрудник
FROM РабочиеМеста
INNER JOIN Сотрудники ON РабочиеМеста.КодСотрудника = Сотрудники.КодСотрудника
WHERE Сотрудники.Состояние=0 and КодРасположения = @КодРасположения
";

        /// <summary>
        ///     Строка запроса: Оборудование в указанном расположении
        /// </summary>
        public const string SELECT_ОборудованиеВРасположении = @"
SELECT	Оборудование.КодОборудования, Оборудование.МодельОборудования, 
	    Сотрудники.Сотрудник, Сотрудники.КодСотрудника,
	    CONVERT(tinyint,CASE WHEN КодТипаОборудования < 100 THEN 1 ELSE 0 END) IT 
FROM	vwОборудованиеСотрудников Оборудование LEFT JOIN 
	    Сотрудники  ON Сотрудники.КодСотрудника = Оборудование.КодСотрудника
WHERE	Оборудование.КодРасположения = @КодРасположения
ORDER BY Оборудование.КодТипаОборудования, Оборудование.МодельОборудования
";

        /// <summary>
        ///     Строка запроса: Розетки в указанном расположении
        /// </summary>
        public const string SELECT_РозеткиВРасположении = @"
IF OBJECT_ID('tempdb..#T') IS NOT NULL DROP TABLE #T
CREATE TABLE #T(КодРозетки int PRIMARY KEY, Розетка varchar(10), Работает tinyint, 
		        Примечание varchar(100), 
		        Подключено nvarchar(MAX),
                КодТелефонногоНомера int,
		        ПодключеноТелефонныйНомер tinyint, 
                КодОборудования int,
		        ПодключеноЛокальнаяСеть tinyint,
		        Изменил int, ИзменилФИО varchar(155), Изменено datetime)

INSERT #T(КодРозетки, Розетка, Работает, Примечание, Изменил, Изменено)
SELECT	КодРозетки, Розетка, CONVERT(tinyint,Работает) Работает, Примечание, Изменил, Изменено 
FROM	vwРозетки WHERE КодРасположения = @Кодрасположения

--Заполняем подключено по телефонным номерам
UPDATE	X
SET Подключено =	RTRIM('Тел: ' + vwТелефонныеНомера.ПолныйНомер + ' ' + 
			        CASE	WHEN vwТелефонныеНомера.КодСотрудника IS NULL 
				            THEN vwТелефонныеНомера.Подключено
				            ELSE Сотрудники.Сотрудник + ISNULL('(' + NULLIF(vwТелефонныеНомера.Подключено,'') + ')','') END),
    ПодключеноТелефонныйНомер = 1,
    КодТелефонногоНомера  = vwТелефонныеНомера.КодТелефонногоНомера
FROM	#T X INNER JOIN 
	    vwТелефонныеНомера ON X.КодРозетки = vwТелефонныеНомера.КодРозетки LEFT JOIN
	    Сотрудники ON vwТелефонныеНомера.КодСотрудника = Сотрудники.КодСотрудника

--Заполняем подключено по локальной сети
UPDATE	X
SET	    Подключено =	vwОборудование.СетевоеИмя + '<' + CONVERT(varchar,vwЛокальнаяСеть.Порт) + '>' + 
		ISNULL(' '  + Сотрудники.Сотрудник,'') + 
		ISNULL(' (' + NULLIF(vwЛокальнаяСеть.Подключено,'') + ')',''),  
	    ПодключеноЛокальнаяСеть = 1,
        КодОборудования = vwОборудование.КодОборудования
FROM	#T X INNER JOIN 
	    vwЛокальнаяСеть ON X.КодРозетки = vwЛокальнаяСеть.КодРозетки INNER JOIN 
	    vwОборудование ON vwЛокальнаяСеть.КодОборудования = vwОборудование.КодОборудования LEFT JOIN
	    Сотрудники ON vwЛокальнаяСеть.КодСотрудника = Сотрудники.КодСотрудника  

UPDATE #T SET Подключено = '' WHERE Подключено IS NULL
UPDATE #T SET ПодключеноТелефонныйНомер = 0 WHERE ПодключеноТелефонныйНомер IS NULL
UPDATE #T SET ПодключеноЛокальнаяСеть = 0 WHERE ПодключеноЛокальнаяСеть IS NULL

UPDATE	X 
SET	    ИзменилФИО = Сотрудник 
FROM	#T X INNER JOIN
	    Сотрудники ON X.Изменил = Сотрудники.КодСотрудника

SELECT * FROM #T ORDER BY КодРозетки

DROP TABLE #T
";

        #endregion

        //++++++++++++++++++ Справочники ++++++++++++++++++

        #region Лица

        /// <summary>
        ///     Строка запроса: Последнее изменение Лица
        /// </summary>
        public const string SELECT_Лицо_LastChanged =
            @"SELECT T0.Изменено FROM vwЛица T0 (nolock) WHERE T0.КодЛица = @id";

        /// <summary>
        ///     Строка запроса: Лица
        /// </summary>
        public const string SELECT_Лица = @"
SELECT {0} T0.КодЛица, T0.Кличка FROM vwЛица T0 (nolock)";

        /// <summary>
        ///     Строка запроса: Получаем бизнес проект по ID
        /// </summary>
        public const string SELECT_ID_БизнесПроект =
            @"SELECT КодБизнесПроекта, БизнесПроект FROM Справочники.dbo.vwБизнесПроекты WHERE КодБизнесПроекта = @id";

        /// <summary>
        ///     Строка запроса: Получение информации о бизнес-проектах для дерева
        /// </summary>
        public const string SELECT_БизнесПроектыДанныеДляДерева = @"
SELECT  r.КодБизнесПроекта Id, r.R-r.L ЕстьДети, r.БизнесПроект Text, ISNULL(r.Parent,0) ParentId, '' Фильтр
FROM vwБизнесПроекты r
WHERE   ((@Потомки = 1 AND ((@Код = 0 AND r.Parent IS NULL) OR(@Код<> 0 AND r.Parent = @Код)))
        OR(@Потомки = 0 AND r.КодБизнесПроекта = @Код)) 
ORDER BY r.{0}";

        /// <summary>
        ///     Строка запроса: Получение информации о бизнес-проектах для дерева, учитывая фильтр
        /// </summary>
        public static string SELECT_БизнесПроектыДанныеДляДерева_Фильтр = @"
DECLARE @МаксимальноеКоличествоНайденных int = 100
SET NOCOUNT ON
               
IF OBJECT_ID('tempdb.#Проекты') IS NOT NULL DROP TABLE #Проекты
CREATE TABLE #Проекты(
        КодЗаписи int IDENTITY(1,1),
        [КодБизнесПроекта] [int],
        БизнесПроект [varchar](50),       
        [Parent] [int],
        [L] [int],
        [R] [int],
        [Изменил] [int],
        [Изменено] [datetime],  
        BitMask tinyint
)

INSERT #Проекты
SELECT  [КодБизнесПроекта],
        БизнесПроект,      
        [Parent],
        [L],
        [R],
        NULL [Изменил],
        NULL [Изменено], 
        1 BitMask
FROM    vwБизнесПроекты 
WHERE   БизнесПроект LIKE '{1}' 
ORDER BY L
 
SET @КоличествоНайденных = @@ROWCOUNT
DELETE #Проекты WHERE КодЗаписи > @МаксимальноеКоличествоНайденных
 
UPDATE  Parent
SET     BitMask = BitMask ^ 2
FROM    #Проекты Parent
WHERE   EXISTS(SELECT * FROM #Проекты Child WHERE Parent.L < Child.L AND Parent.R > Child.R)
 
INSERT  #Проекты
SELECT  [КодБизнесПроекта],
        БизнесПроект,      
        [Parent],
        [L],
        [R],
        NULL [Изменил],
        NULL [Изменено], 
        2 BitMask
FROM    vwБизнесПроекты  Parent 
WHERE   EXISTS( SELECT * FROM #Проекты Child 
                WHERE Parent.L <= Child.L AND Parent.R>=Child.R)                                        
        AND NOT EXISTS(SELECT * FROM #Проекты X WHERE Parent.КодБизнесПроекта = X.КодБизнесПроекта) 
 
UPDATE  #Проекты
SET     BitMask = BitMask ^ 4
WHERE   Parent IS NULL
 
INSERT #Проекты 
SELECT  [КодБизнесПроекта],
        БизнесПроект,      
        [Parent],
        [L],
        [R],
        NULL [Изменил],
        NULL [Изменено], 
        4 BitMask
FROM    vwБизнесПроекты 
WHERE   Parent IS NULL                                  
        AND NOT EXISTS(SELECT * FROM #Проекты X WHERE vwБизнесПроекты.КодБизнесПроекта = X.КодБизнесПроекта)
        
SELECT [КодБизнесПроекта] id,
       БизнесПроект text,      
       [Parent] ParentId,
       [L],
       [R],
       [Изменил],
       [Изменено], 
       BitMask,
       R-L ЕстьДети
FROM #Проекты
ORDER BY {0}
DROP TABLE #Проекты
";
            /// <summary>
            /// Строка запроса: Список бизнес-проектов для дерева
            /// </summary>
        public static string SELECT_БизнесПроектыДанныеДляДерева_State = @"
SET NOCOUNT ON

IF OBJECT_ID('tempdb.#Проекты') IS NOT NULL DROP TABLE #Проекты
CREATE TABLE #Проекты(
        КодЗаписи int IDENTITY(1,1),
        [КодБизнесПроекта] [int],
        БизнесПроект [varchar](300),       
        [Parent] [int],
        [L] [int],
        [R] [int],
        [Изменил] [int],
        [Изменено] [datetime],  
        BitMask tinyint
)

INSERT #Проекты
SELECT	Parent.[КодБизнесПроекта],
        Parent.БизнесПроект,      
        Parent.[Parent],
        Parent.[L],
        Parent.[R],
        NULL [Изменил],
        NULL [Изменено], 
        1 BitMask
FROM	vwБизнесПроекты Parent 
WHERE EXISTS(SELECT * FROM vwБизнесПроекты Child 
					WHERE	Child.КодБизнесПроекта IN ({1})
						AND Parent.L <=	Child.L AND Parent.R>=Child.R) 
      AND NOT EXISTS(SELECT * FROM #Проекты X WHERE Parent.КодБизнесПроекта = X.КодБизнесПроекта)
ORDER BY Parent.L

INSERT #Проекты
SELECT	Child.[КодБизнесПроекта],
        Child.БизнесПроект,      
        Child.[Parent],
        Child.[L],
        Child.[R],
        NULL [Изменил],
        NULL [Изменено], 
        2 BitMask
FROM	vwБизнесПроекты Parent
LEFT JOIN vwБизнесПроекты Child ON Child.Parent = Parent.КодБизнесПроекта
WHERE Parent.КодБизнесПроекта IN ({1})  AND NOT EXISTS(SELECT * FROM #Проекты X WHERE Child.КодБизнесПроекта = X.КодБизнесПроекта)
ORDER BY Parent.L

INSERT #Проекты 
SELECT  [КодБизнесПроекта],
        БизнесПроект,      
        [Parent],
        [L],
        [R],
        NULL [Изменил],
        NULL [Изменено], 
        4 BitMask
FROM    vwБизнесПроекты 
WHERE   Parent IS NULL                                  
        AND NOT EXISTS(SELECT * FROM #Проекты X WHERE vwБизнесПроекты.КодБизнесПроекта = X.КодБизнесПроекта) 

SELECT [КодБизнесПроекта] id,
       БизнесПроект text,      
       [Parent] ParentId,
       [L],
       [R],
       [Изменил],
       [Изменено], 
       BitMask,
       R-L ЕстьДети
FROM #Проекты
ORDER BY {0}
DROP TABLE #Проекты
";

        /// <summary>
        ///     Строка запроса: Удаление бизнес-проекта
        /// </summary>
        public const string DELETE_БизнесПроект =
            @"UPDATE БизнесПроекты SET Закрыт = 1 WHERE КодБизнесПроекта=@КодБизнесПроекта";

        /// <summary>
        ///     Строка запроса: Получение информации о тестовых записях для дерева
        /// </summary>
        public const string SELECT_ТестовыеЗаписиДляДерева = @"
SELECT  r.КодЗаписи Id, r.R-r.L ЕстьДети, r.Название Text, ISNULL(r.Parent,0) ParentId, '' Фильтр
FROM Записи r
WHERE   ((@Потомки = 1 AND ((@Код = 0 AND r.Parent IS NULL) OR(@Код<> 0 AND r.Parent = @Код)))
        OR(@Потомки = 0 AND r.КодЗаписи = @Код)) 
ORDER BY r.{0}";

        /// <summary>
        ///     Строка запроса: Получаем организационно правовую форму по ID
        /// </summary>
        public const string SELECT_ID_ОрганизационноПравоваяФорма =
            @"SELECT * FROM Справочники.dbo.ОргПравФормы WHERE КодОргПравФормы = @id";

        /// <summary>
        ///     Строка запроса: Поиск лица по коду
        /// </summary>
        public const string SELECT_ID_Лицо = @"
SELECT  T0.КодЛица, T0.ТипЛица, T0.КодБизнесПроекта, T0.Проверено, T0.Кличка, T0.КличкаRL, T0.НазваниеRL, T0.КодТерритории,
        T0.ГосОрганизация, T0.ИНН, T0.ОГРН, T0.ОКПО, T0.БИК, T0.КорСчет, T0.БИКРКЦ, T0.SWIFT, T0.ДатаРождения,T0.ДатаКонца, T0.Примечание, T0.Изменено
FROM    vwЛица T0 (nolock)
WHERE   T0.КодЛица = @id";

        /// <summary>
        ///     Строка запроса: Признак ЛицоДействует
        /// </summary>
        public const string SELECT_CHECK_ЛицоДействует = @"
IF EXISTS (SELECT NULL FROM fn_ДействующиеЛица(@Дата) WHERE @КодЛица=КодЛица)
SELECT 1
ELSE
SELECT 0";

        /// <summary>
        ///     Строка запроса: Получить карточку физического лица по ID
        /// </summary>
        public const string SELECT_ID_КарточкаФизЛица =
            "SELECT * FROM vwКарточкиФизЛиц (nolock) WHERE КодКарточкиФизЛица = @id";

        /// <summary>
        ///     Строка запроса: Получить карточку юридического лица по ID
        /// </summary>
        public const string SELECT_ID_КарточкаЮрЛица =
            "SELECT * FROM vwКарточкиЮрЛиц (nolock) WHERE КодКарточкиЮрЛица=@id";

        /// <summary>
        ///     Строка запроса: Получить карточки физического лица по ID
        /// </summary>
        public const string SELECT_ID_КарточкиФизЛица =
            "SELECT * FROM vwКарточкиФизЛиц (nolock) WHERE КодЛица = @id ORDER BY От";

        /// <summary>
        ///     Строка запроса: Получить карточки юридического лица по ID
        /// </summary>
        public const string SELECT_ID_КарточкиЮрЛица =
            "SELECT * FROM vwКарточкиЮрЛиц (nolock) WHERE КодЛица = @id ORDER BY От";

        /// <summary>
        ///     Строка запроса: Получаем бизнес проекты по имени
        /// </summary>
        public const string SELECT_БизнесПроектыПоИмени = @"
DECLARE @pattern varchar(100)
SET @pattern = Инвентаризация.dbo.fn_ReplaceRusLat(Инвентаризация.dbo.fn_ReplaceKeySymbols(@search))+'%'
SET @pattern = COALESCE(@pattern, '')
SELECT * FROM vwБизнесПроекты 
WHERE @pattern = '' OR Инвентаризация.dbo.fn_ReplaceRusLat(Инвентаризация.dbo.fn_ReplaceKeySymbols(БизнесПроект)) LIKE @pattern
ORDER BY БизнесПроект";

        /// <summary>
        ///     Строка запроса: Получаем организационно правовые формы по имени
        /// </summary>
        public const string SELECT_ОргПравФормуПоИмени = @"
DECLARE @pattern varchar(100)

SET @personKind = COALESCE(@personKind,0)
SET @pattern = Инвентаризация.dbo.fn_ReplaceRusLat(Инвентаризация.dbo.fn_ReplaceKeySymbols(@search))+'%'
SET @pattern = COALESCE(@pattern, '')

SELECT  * FROM ОргПравФормы 
WHERE   (@pattern = '' OR Инвентаризация.dbo.fn_ReplaceRusLat(ОргПравФорма) LIKE @pattern OR Инвентаризация.dbo.fn_ReplaceRusLat(КраткоеНазвание) LIKE @pattern) 
        AND (@personKind = 0 OR ТипЛица = @personKind)
ORDER BY ОргПравФорма";

        /// <summary>
        ///     Строка запроса: Поиск отвественных сотрудников по ID лица
        /// </summary>
        public const string SELECT_ОтвественныеСотрудники_ПоЛицу = @"
SELECT  * 
FROM    Инвентаризация.dbo.Сотрудники
WHERE   КодСотрудника IN (SELECT КодСотрудника FROM vwЛица_Сотрудники WHERE КодЛица = @id)
ORDER BY Сотрудник";


        /// <summary>
        ///     Строка запроса: Добавление ответственных сотрудников + удаление неиспользуемых
        /// </summary>
        public const string UPDATE_ОтвественныеСотрудники_ПоЛицу = @"
DECLARE @Коды table(КодСотрудника int)
INSERT @Коды SELECT * FROM Инвентаризация.dbo.fn_SplitInts(@ids)

INSERT INTO vwЛица_Сотрудники(КодЛица,КодСотрудника)
SELECT  @personID, Коды.КодСотрудника 
FROM    @Коды Коды INNER JOIN 
        Инвентаризация.dbo.Сотрудники Empl ON Empl.КодСотрудника = Коды.КодСотрудника
WHERE NOT EXISTS( SELECT * FROM vwЛица_Сотрудники WHERE vwЛица_Сотрудники.КодСотрудника = Коды.КодСотрудника AND vwЛица_Сотрудники.КодЛица = @personID )
			
IF (EXISTS(SELECT * FROM vwЛица_Сотрудники WHERE КодЛица = @personID AND КодСотрудника NOT IN ( SELECT КодСотрудника FROM @Коды )))
	DELETE FROM vwЛица_Сотрудники 
    WHERE КодЛица = @personID AND КодСотрудника NOT IN ( SELECT КодСотрудника FROM @Коды )";


        /// <summary>
        ///     Строка запроса: Поиск атрибута лица по ID
        /// </summary>
        public const string SELECT_ID_Атрибут = @"
SELECT  КодЛица, КодТипаАтрибута, КодАтрибутовЛиц, КодТерритории, ДатаНачалаДействия, ДатаОкончанияДействия, КодТерриторииФормата,
        ИмяАтрибутаРус1, ИмяАтрибутаРус2, ИмяАтрибутаРус3,
        ИмяАтрибутаЛат1, ИмяАтрибутаЛат2, ИмяАтрибутаЛат3,
        ИмяАтрибутаНаЯзыкеСтраны1, ИмяАтрибутаНаЯзыкеСтраны2, ИмяАтрибутаНаЯзыкеСтраны3,
        ЗначениеАтрибута1, ЗначениеАтрибута2, ЗначениеАтрибута3, 
        ФорматАтрибута1, ФорматАтрибута2, ФорматАтрибута3,
        ДатаРождения, ДатаКонца, УникаленВПределахТерритории, ПорядокВыводаАтрибута, КодФорматаАтрибута,
        Изменил, Изменено, Проверено
FROM    vwЛицаАтрибуты (nolock) WHERE КодАтрибутовЛиц = @id";


        /// <summary>
        ///     Строка запроса: Поиск атрибутов лица по ID лица
        /// </summary>
        public const string SELECT_АтрибутыЛица = @"
SELECT  КодЛица, КодТипаАтрибута, КодАтрибутовЛиц, КодТерритории, ДатаНачалаДействия, ДатаОкончанияДействия, КодТерриторииФормата,
		ИмяАтрибутаРус1, ИмяАтрибутаРус2, ИмяАтрибутаРус3,
		ИмяАтрибутаЛат1, ИмяАтрибутаЛат2, ИмяАтрибутаЛат3,
		ИмяАтрибутаНаЯзыкеСтраны1, ИмяАтрибутаНаЯзыкеСтраны2, ИмяАтрибутаНаЯзыкеСтраны3,
		ЗначениеАтрибута1, ЗначениеАтрибута2, ЗначениеАтрибута3, 
		ФорматАтрибута1, ФорматАтрибута2, ФорматАтрибута3,
		ДатаРождения, ДатаКонца, УникаленВПределахТерритории, ПорядокВыводаАтрибута, КодФорматаАтрибута,
		Изменил, Изменено
FROM    vwЛицаАтрибуты (nolock) WHERE КодЛица = @id";


        /// <summary>
        ///     Строка запроса: Поиск формата атрибута по ID
        /// </summary>
        public const string SELECT_ID_ФорматАтрибута = @"
SELECT  ИмяАтрибутаРус1, ИмяАтрибутаРус2, ИмяАтрибутаРус3,
        ИмяАтрибутаЛат1, ИмяАтрибутаЛат2, ИмяАтрибутаЛат3,
        ИмяАтрибутаНаЯзыкеСтраны1, ИмяАтрибутаНаЯзыкеСтраны2, ИмяАтрибутаНаЯзыкеСтраны3,
        ФорматАтрибута1, ФорматАтрибута2, ФорматАтрибута3,
        КодФорматаАтрибута, КодТерритории, ТипЛица, КодТипаАтрибута, УникаленВПределахТерритории,
        ПроверяемыйАтрибут, Изменено, Изменил
FROM    vwФорматАтрибута (nolock) WHERE КодФорматаАтрибута = @id";

        /// <summary>
        ///     Строка запроса: Поиск формата атрибута по типу формата атрибута и по территории
        /// </summary>
        public const string SELECT_ФорматаАтрибута_Территория_ТипЛица_ТипАтрибута = @"
SELECT  ИмяАтрибутаРус1, ИмяАтрибутаРус2, ИмяАтрибутаРус3,
		ИмяАтрибутаЛат1, ИмяАтрибутаЛат2, ИмяАтрибутаЛат3,
		ИмяАтрибутаНаЯзыкеСтраны1, ИмяАтрибутаНаЯзыкеСтраны2, ИмяАтрибутаНаЯзыкеСтраны3,
		ФорматАтрибута1, ФорматАтрибута2, ФорматАтрибута3,
		КодФорматаАтрибута, КодТерритории, ТипЛица, КодТипаАтрибута, УникаленВПределахТерритории,
        ПроверяемыйАтрибут, Изменено, Изменил
FROM    vwФорматАтрибута (nolock) 
WHERE КодТерритории = @territoryID AND ТипЛица = @personType AND КодТипаАтрибута = @formatTypeID";


        /// <summary>
        ///     Строка запроса: Поиск формата атрибута по умолчанию
        /// </summary>
        public const string SELECT_ФорматаАтрибутаПоУмолчанию = @"
SELECT  ИмяАтрибутаРус1, ИмяАтрибутаРус2, ИмяАтрибутаРус3,
        ИмяАтрибутаЛат1, ИмяАтрибутаЛат2, ИмяАтрибутаЛат3,
        CASE    WHEN ИмяАтрибутаРус2 = '' THEN 1 
                WHEN ИмяАтрибутаРус3 = '' THEN 2  
                ELSE 3 END КоличествоПолей
FROM vwФорматАтрибута 
WHERE КодТерритории IS NULL AND КодТипаАтрибута = @formatTypeID AND ТипЛица = @personType";

        /// <summary>
        ///     Строка запроса: Поиск типа формата атрибута по коду
        /// </summary>
        public const string SELECT_ID_ТипАтрибута = @"
SELECT  КодТипаАтрибута, ТипАтрибута, ДоступностьДляТипаЛица, ПорядокВыводаАтрибута, ТипАтрибутаЛат
FROM    vwТипАтрибута (nolock) WHERE КодТипаАтрибута = @id";

        /// <summary>
        ///     Строка запроса: Получаем типы форматов атрибутов
        /// </summary>
        public const string SELECT_ТипыАтрибутов = @"
SELECT  КодТипаАтрибута, ТипАтрибута, ДоступностьДляТипаЛица, ПорядокВыводаАтрибута, ТипАтрибутаЛат
FROM    vwТипАтрибута";


        /// <summary>
        ///     Хранимая процедура: Получение пунктов меню досье лица
        /// </summary>
        public const string SP_Лица_Досье_Меню = "sp_Лица_Досье_$Меню";

        /// <summary>
        ///     Хранимая процедура: Получить досье лица
        /// </summary>
        public const string SP_Лица_Досье_Context_NEW = "sp_Лица_Досье_Context_new";

        /// <summary>
        ///     Хранимая процедура: Поиск лиц
        /// </summary>
        public const string SP_Лица_Поиск = "sp_Лица_Поиск";

        /// <summary>
        ///     Хранимая процедура: Создание физического лица
        /// </summary>
        public const string SP_Лица_ФизическоеЛицо_Ins = "sp_Лица_ФизическоеЛицо_Ins";

        /// <summary>
        ///     Хранимая процедура: Создание физического лица
        /// </summary>
        public const string SP_Лица_ЮридическоеЛицо_Ins = "sp_Лица_ЮридическоеЛицо_Ins";

        /// <summary>
        ///     Хранимая процедура: Редактирования атрибута
        /// </summary>
        public const string SP_Лица_Upd = "sp_Лица_Upd";

        /// <summary>
        ///     Хранимая процедура: Сохранение изменений в формате атрибута
        /// </summary>
        public const string SP_Лица_ФорматАтрибута_Upd = "sp_Лица_ФорматАтрибута_Upd";

        /// <summary>
        ///     Хранимая процедура: Создание формата атрибута
        /// </summary>
        public const string SP_Лица_ФорматАтрибута_Ins = "sp_Лица_ФорматАтрибута_Ins";

        /// <summary>
        ///     Хранимая процедура: Создание атрибута
        /// </summary>
        public const string SP_Лица_Атрибут_Ins = "sp_Лица_Атрибут_Ins";

        /// <summary>
        ///     Хранимая процедура: Удаление атрибута
        /// </summary>
        public const string SP_Лица_Атрибут_Del = "sp_Лица_Атрибут_Del";

        /// <summary>
        ///     Хранимая процедура: Редактирование атрибута
        /// </summary>
        public const string SP_Лица_Атрибут_Upd = "sp_Лица_Атрибут_Upd";

        /// <summary>
        ///     Хранимая процедура: Проверка атрибута
        /// </summary>
        public const string SP_Лица_Атрибут_Check = "sp_Лица_Атрибут_Check";

        /// <summary>
        ///     Получение должности физического лица
        /// </summary>
        public const string SELECT_ДолжностьФизическогоЛица = @"
SELECT  Описание 
FROM    vwСвязиЛиц 
WHERE   КодЛицаРодителя = @КодЛицаРодителя 
        AND КодЛицаПотомка = @КодЛицаПотомка 
        AND Параметр = @Параметр 
        AND КодТипаСвязиЛиц = @КодТипаСвязиЛиц
        AND От <= @Дата AND @Дата < До
";

        /// <summary>
        ///     Строка запроса: Проверка наличия у лица подписанта с указанным типом подписи
        /// </summary>
        public const string SELECT_ЛицоИмеетПодписантовСПравомПодписи = @"
IF EXISTS(SELECT  * 
FROM    vwСвязиЛиц 
WHERE   КодЛицаРодителя = @КодЛицаРодителя        
        AND Параметр = @Параметр 
        AND КодТипаСвязиЛиц = @КодТипаСвязиЛиц
        AND От <= @Дата AND @Дата < До)
    SELECT 1
ELSE
    SELECT 0";

        #endregion

        #region Связи лиц

        /// <summary>
        ///     Хранимая процедура: Cоздание, изменение, удаления связи лиц
        /// </summary>
        public const string SP_Лица_InsUpdDel_СвязиЛиц = "sp_Лица_InsUpdDel_СвязиЛиц";

        /// <summary>
        ///     Строка запроса: Поиск связи лиц по ID
        /// </summary>
        public const string SELECT_СвязиЛиц = @"
SELECT  КодСвязиЛиц, КодТипаСвязиЛиц, От, До, КодЛицаРодителя, КодЛицаПотомка, COALESCE(NULLIF(Описание,'') , '<Нет описания>') as Описание,
        Параметр, Изменил, Изменено
FROM    vwСвязиЛиц  (nolock)
";

        /// <summary>
        ///     Строка запроса: Поиск связи лиц по ID
        /// </summary>
        public const string SELECT_ID_СвязиЛиц = @"
SELECT  КодСвязиЛиц, КодТипаСвязиЛиц, От, До, КодЛицаРодителя, КодЛицаПотомка, COALESCE(NULLIF(Описание,'') , '<Нет описания>') as Описание,
        Параметр, Изменил, Изменено 
FROM    vwСвязиЛиц (nolock) 
WHERE   КодСвязиЛиц = @id";

        /// <summary>
        ///     Строка запроса: Поиск связи лиц типу связи лиц
        /// </summary>
        public const string SELECT_СвязиЛиц_ПоТипуСвязи = @"
SELECT  КодСвязиЛиц, КодТипаСвязиЛиц, От, До, КодЛицаРодителя, КодЛицаПотомка, COALESCE(NULLIF(Описание,'') , '<Нет описания>') as Описание,
        Параметр, Изменил, Изменено 
FROM    vwСвязиЛиц (nolock) 
WHERE   КодТипаСвязиЛиц = @LinkTypeID";

        /// <summary>
        ///     Строка запроса: Поиск связи лиц по ID родителя
        /// </summary>
        public const string SELECT_СвязиЛиц_ПоРодителю = @"
SELECT  КодСвязиЛиц, КодТипаСвязиЛиц, От, До, КодЛицаРодителя, КодЛицаПотомка, COALESCE(NULLIF(Описание,'') , '<Нет описания>') as Описание,
        Параметр, Изменил, Изменено 
FROM    vwСвязиЛиц (nolock) 
WHERE   КодЛицаРодителя = @ParentID";


        /// <summary>
        ///     Строка запроса: Поиск связи лиц по ID подчиненного
        /// </summary>
        public const string SELECT_СвязьЛиц_ПоПотомку = @"
SELECT  КодСвязиЛиц, КодТипаСвязиЛиц, От, До, КодЛицаРодителя, КодЛицаПотомка, COALESCE(NULLIF(Описание,'') , '<Нет описания>') as Описание,
        Параметр, Изменил, Изменено 
FROM    vwСвязиЛиц (nolock) 
WHERE   КодЛицаПотомка = @ChildID";

        /// <summary>
        ///     Строка запроса: Поиск связи лиц по ID родителя и типу связи лиц
        /// </summary>
        public const string SELECT_СвязиЛиц_ПоРодителю_ПоТипуСвязи = @"
SELECT  КодСвязиЛиц, КодТипаСвязиЛиц, От, До, КодЛицаРодителя, КодЛицаПотомка, COALESCE(NULLIF(Описание,'') , '<Нет описания>') as Описание,
        Параметр, Изменил, Изменено 
FROM    vwСвязиЛиц (nolock)  
WHERE   КодЛицаРодителя = @ParentID AND КодТипаСвязиЛиц = @LinkTypeID";


        /// <summary>
        ///     Строка запроса: Поиск связи лиц по ID подчиненногои и типу связи лиц
        /// </summary>
        public const string SELECT_СвязиЛиц_ПоПотомку_ПоТипуСвязи = @"
SELECT  КодСвязиЛиц, КодТипаСвязиЛиц, От, До, КодЛицаРодителя, КодЛицаПотомка, COALESCE(NULLIF(Описание,'') , '<Нет описания>') as Описание,
        Параметр, Изменил, Изменено 
FROM    vwСвязиЛиц (nolock)
WHERE КодЛицаПотомка = @ChildID AND КодТипаСвязиЛиц = @LinkTypeID";

        /// <summary>
        ///     Строка запроса: Поиск связей лиц по ID родителя и ID подчиненного
        /// </summary>
        public const string SELECT_СвязиЛиц_ПоРодителю_ПоПотомку = @"
SELECT  КодСвязиЛиц, КодТипаСвязиЛиц, От, До, КодЛицаРодителя, КодЛицаПотомка, COALESCE(NULLIF(Описание,'') , '<Нет описания>') as Описание,
        Параметр, Изменил, Изменено
FROM    vwСвязиЛиц (nolock)
WHERE   КодЛицаРодителя = @ParentID AND КодЛицаПотомка = @ChildID";

        /// <summary>
        ///     Строка запроса: Поиск связей лиц по ID родителя и ID подчиненного и типу связи лиц
        /// </summary>
        public const string SELECT_СвязиЛиц_ПоРодителю_ПоПотомку_ПоТипуСвязи = @"
SELECT  КодСвязиЛиц, КодТипаСвязиЛиц, От, До, КодЛицаРодителя, КодЛицаПотомка, COALESCE(NULLIF(Описание,'') , '<Нет описания>') as Описание,
        Параметр, Изменил, Изменено
FROM    vwСвязиЛиц (nolock)
WHERE   КодЛицаРодителя = @ParentID AND КодЛицаПотомка = @ChildID AND КодТипаСвязиЛиц = @LinkTypeID";

        /// <summary>
        ///     Строка запроса: Поиск связи лиц по ID лица
        /// </summary>
        public const string SELECT_СвязиЛиц_МестаРаботы_Работники_ПоЛицу = @"
SELECT  КодСвязиЛиц, КодТипаСвязиЛиц, От, До, КодЛицаРодителя, КодЛицаПотомка, COALESCE(NULLIF(Описание,'') , '<Нет описания>') as Описание,
        Параметр, Изменил, Изменено 
FROM    vwСвязиЛиц (nolock)
WHERE   КодТипаСвязиЛиц = 1 AND @personID IN(КодЛицаРодителя, КодЛицаПотомка)";

        /// <summary>
        ///     Строка запроса: Поиск работников по месту работы
        /// </summary>
        public const string SELECT_СвязиЛиц_Работники_ПоЛицу = @"
SELECT  КодСвязиЛиц, КодТипаСвязиЛиц, От, До, КодЛицаРодителя, КодЛицаПотомка, COALESCE(NULLIF(Описание,'') , '<Нет описания>') as Описание,
        Параметр, Изменил, Изменено 
FROM    vwСвязиЛиц (nolock)
WHERE   КодТипаСвязиЛиц = 1 AND КодЛицаРодителя = @personID";

        /// <summary>
        ///     Строка запроса: Поиск мест работы по работнику
        /// </summary>
        public const string SELECT_СвязиЛиц_МестаРаботы_ПоЛицу = @"
SELECT  КодСвязиЛиц, КодТипаСвязиЛиц, От, До, КодЛицаРодителя, КодЛицаПотомка, COALESCE(NULLIF(Описание,'') , '<Нет описания>') as Описание,
        Параметр, Изменил, Изменено 
FROM    vwСвязиЛиц  (nolock)
WHERE   КодТипаСвязиЛиц = 1 AND КодЛицаПотомка= @personID";

        #endregion

        #region Типы лиц

        /// <summary>
        ///     Хранимая процедура: Создание и удаление типов лиц
        /// </summary>
        public const string SP_Лица_InsDel_ТипыЛиц = "sp_Лица_InsDel_ТипыЛиц";

        /// <summary>
        ///     Строка запроса: Получить темы лиц
        /// </summary>
        public const string SELECT_ТемыЛиц = @"
SELECT * 
FROM    (SELECT  КодТемыЛица, ТемаЛица, Parent, L, R FROM vwТемыЛиц_Администрирование WHERE КодТемыЛица IS NOT NULL
        UNION
        SELECT  КодТемыЛица, ТемаЛица, Parent, L, R FROM vwТемыЛиц_Tree
        WHERE   КодТемыЛица NOT IN (SELECT КодТемыЛица FROM vwТемыЛиц_Администрирование WHERE КодТемыЛица IS NOT NULL)) T0";

        /// <summary>
        ///     Строка запроса: Получить темы лиц
        /// </summary>
        public const string SELECT_ID_ТемаЛица = @"
SELECT * 
FROM    (SELECT  КодТемыЛица, ТемаЛица, Parent FROM vwТемыЛиц_Администрирование WHERE КодТемыЛица = @id
        UNION
        SELECT  КодТемыЛица, ТемаЛица, Parent FROM vwТемыЛиц_Tree
        WHERE   КодТемыЛица NOT IN (SELECT КодТемыЛица FROM vwТемыЛиц_Администрирование WHERE КодТемыЛица IS NOT NULL) AND КодТемыЛица = @id) T0";

        /// <summary>
        ///     Строка запроса: Получить темы лиц по кодам тем
        /// </summary>
        public const string SELECT_ТипыЛиц_Темы = @"
SELECT  ТиЛ.КодТипаЛица, ТеЛ.КодТемыЛица, ТеЛ.ТемаЛица, К.КодКаталога, К.Каталог
FROM    vwТипыЛицСотрудника ТиЛ INNER JOIN 
        vwТемыЛиц ТеЛ ON ТиЛ.КодТемыЛица=ТеЛ.КодТемыЛица INNER JOIN 
        Каталоги К ON ТиЛ.КодКаталога=К.КодКаталога
WHERE ТеЛ.КодТемыЛица IN (SELECT value FROM Инвентаризация.dbo.fn_SplitInts(@id))
ORDER BY ТемаЛица, Каталог";

        /// <summary>
        ///     Строка запроса: Получить тип лица по ID
        /// </summary>
        public const string SELECT_ID_ТипЛица = @"
SELECT  ТиЛ.КодТипаЛица, ТиЛ.КодКаталога, ТиЛ.КодТемыЛица, К.Каталог
FROM    vwТипыЛицСотрудника ТиЛ INNER JOIN 
        Каталоги К ON ТиЛ.КодКаталога=К.КодКаталога
WHERE   КодТипаЛица = @id";

        /// <summary>
        ///     Строка запроса: Получить типы лиц
        /// </summary>
        public const string SELECT_ТипыЛица = @"SELECT КодТипаЛица, КодКаталога, КодТемыЛица FROM vwТипыЛицСотрудника";

        /// <summary>
        ///     Строка запроса: Получить типы лица по ID лица
        /// </summary>
        public const string SELECT_ТипыЛиц_Лицо = @"
SELECT  КодТипаЛица, ТеЛ.КодТемыЛица, ТеЛ.ТемаЛица, К.КодКаталога, К.Каталог, ТеЛ.L
FROM    vwТипыЛицСотрудника Тил INNER JOIN 
        vwТемыЛиц ТеЛ ON ТиЛ.КодТемыЛица=ТеЛ.КодТемыЛица INNER JOIN 
        Каталоги К ON Тил.КодКаталога=К.КодКаталога
WHERE   КодТипаЛица IN( SELECT КодТипаЛица FROM Лица_ТипыЛиц WHERE КодЛица = @id AND Сотрудник = 0 )
ORDER BY ТемаЛица, Каталог";

        /// <summary>
        ///     Строка запроса: ТемыЛиц_ПотомкиИПодчиненные
        /// </summary>
        public const string SELECT_ID_ТемыЛиц_ПотомкиИПодчиненные = @"
--Проверяем наличие подчиненных и родителей в выбранном списке
CREATE TABLE #T(КодТемыЛица int PRIMARY KEY, ТемаЛица varchar(100), Parent int, L int, R int)
DECLARE @L int, @R int

INSERT	#T(КодТемыЛица, ТемаЛица, Parent, L, R)
SELECT	КодТемыЛица, ТемаЛица, Parent, L, R 
FROM    (SELECT  КодТемыЛица, ТемаЛица, Parent, L, R FROM vwТемыЛиц_Администрирование WHERE КодТемыЛица IS NOT NULL
        UNION
        SELECT  КодТемыЛица, ТемаЛица, Parent, L, R FROM vwТемыЛиц_Tree
        WHERE   КодТемыЛица NOT IN (SELECT КодТемыЛица FROM vwТемыЛиц_Администрирование WHERE КодТемыЛица IS NOT NULL)) T0
WHERE	@КодКаталога = 0 OR EXISTS(SELECT * FROM ТипыЛиц (nolock) WHERE КодКаталога = @КодКаталога AND T0.КодТемыЛица = ТипыЛиц.КодТемыЛица)

SELECT @L = L, @R = R FROM #T WHERE КодТемыЛица = @id 
SELECT	КодТемыЛица, ТемаЛица, 0 Parent FROM #T WHERE @L < L AND @R > R
UNION 
SELECT	КодТемыЛица, ТемаЛица, 1 Parent FROM #T WHERE L < @L AND R > @L
	
DROP TABLE #T
";


        /// <summary>
        ///     Строка запроса: Получить каталог по ID
        /// </summary>
        public const string SELECT_ID_Каталог = @"
SELECT * FROM Каталоги WHERE КодКаталога = @id";

        /// <summary>
        ///     Строка запроса: Список каталогов
        /// </summary>
        public const string SELECT_Каталоги = @"
SELECT * FROM Каталоги";

        #endregion

        #region Контакты

        /// <summary>
        ///     Хранимая процедура: Получаем контакты лица
        /// </summary>
        public const string SP_Лица_Контакты = "sp_Лица_Контакты";

        /// <summary>
        ///     Получаем пустой результат без ошибок в случае добавления условий поиска адреса лица
        /// </summary>
        public const string SELECT_ПустыеКонтактыЛица =
            @"SELECT КодКонтакта, КодТипаКонтакта, Контакт, КодЛица, От, До FROM (VALUES (NULL,NULL,NULL,NULL,NULL,NULL)) AS T0(КодКонтакта,КодТипаКонтакта,Контакт,КодЛица,От,До) WHERE КодКонтакта IS NOT NULL";

        /// <summary>
        ///     Получаем адреса лица
        /// </summary>
        public const string SELECT_КонтактыЛица = @"
SELECT DISTINCT {0} КодКонтакта, Контакт FROM
(SELECT '1' + CONVERT(varchar, КодКонтакта) AS КодКонтакта, КодТипаКонтакта, Контакт, КодЛица, NULL AS От, NULL AS До FROM vwКонтакты WHERE @ИсточникКонтакта&1=1
UNION ALL
SELECT '2' + CONVERT(varchar, КодКарточкиЮрЛица) AS КодКонтакта, 1 AS КодТипаКонтакта, АдресЮридический Контакт, КодЛица, От, До FROM vwКарточкиЮрЛиц (nolock) WHERE @ИсточникКонтакта&2=2
UNION ALL
SELECT '4'+ CONVERT(varchar, КодКарточкиФизЛица) AS КодКонтакта, 1 AS КодТипаКонтакта, АдресЮридический Контакт, КодЛица, От, До FROM vwКарточкиФизЛиц (nolock) WHERE @ИсточникКонтакта&4=4) AS T0
WHERE
Контакт<>''";

        /// <summary>
        ///     Получаем адреса лица по указанному условному коду, возвращаемому как КодКонтакта из предыдущего запроса
        /// </summary>
        public const string SELECT_ID_КонтактыЛица = @"
SELECT TOP 1 КодТипаКонтакта+CONVERT(varchar, КодКонтакта) AS КодКонтакта, Контакт FROM
(
SELECT КодКонтакта, '1' AS КодТипаКонтакта, Контакт, КодЛица, NULL AS От, NULL AS До FROM vwКонтакты WHERE 1=CAST(LEFT(@КодКонтакта,1) as int)&1
UNION ALL
SELECT КодКарточкиЮрЛица AS КодКонтакта, '2' AS КодТипаКонтакта, АдресЮридический Контакт, КодЛица, От, До FROM vwКарточкиЮрЛиц (nolock) WHERE 2=CAST(LEFT(@КодКонтакта,1) as int)&2
UNION ALL
SELECT КодКарточкиФизЛица AS КодКонтакта, '4' AS КодТипаКонтакта, АдресЮридический Контакт, КодЛица, От, До FROM vwКарточкиФизЛиц (nolock) WHERE 4=CAST(LEFT(@КодКонтакта,1) as int)&4) AS T0
WHERE
КодКонтакта = CAST( SUBSTRING (@КодКонтакта, 2, 30) as int)
ORDER BY (SELECT NULL)";

        /// <summary>
        ///     Строка запроса: признак - КонтактЛица
        /// </summary>
        public const string SELECT_TEST_КонтактЛица = @"
IF EXISTS
(SELECT NULL FROM
(SELECT КодКонтакта, КодЛица FROM vwКонтакты WHERE 1=CAST(LEFT(@КодКонтакта,1) as int)&1
UNION ALL
SELECT КодКарточкиЮрЛица AS КодКонтакта, КодЛица FROM vwКарточкиЮрЛиц (nolock) WHERE 2=CAST(LEFT(@КодКонтакта,1)&2 as int)&2
UNION ALL
SELECT КодКарточкиФизЛица AS КодКонтакта, КодЛица FROM vwКарточкиФизЛиц (nolock) WHERE 4=CAST(LEFT(@КодКонтакта,1) as int)&4) AS T0
WHERE
КодКонтакта= CAST( SUBSTRING (@КодКонтакта, 2, 30) as int)
AND КодЛица= @КодЛица)
SELECT 1
ELSE
SELECT 0";

        /// <summary>
        ///     Строка запроса: Возвращает признак действующего контракта (0/1)
        /// </summary>
        public const string SELECT_TEST_КонтактДействует = @"
IF EXISTS
(SELECT NULL FROM
(SELECT КодКонтакта, NULL AS От, NULL AS До FROM vwКонтакты WHERE 1=CAST(LEFT(@КодКонтакта,1) as int)&1
UNION ALL
SELECT КодКарточкиЮрЛица AS КодКонтакта, От, До FROM vwКарточкиЮрЛиц (nolock) WHERE 2=CAST(LEFT(@КодКонтакта,1)&2 as int)&2
UNION ALL
SELECT КодКарточкиФизЛица AS КодКонтакта, От, До FROM vwКарточкиФизЛиц (nolock) WHERE 4=CAST(LEFT(@КодКонтакта,1) as int)&4) AS T0
WHERE
КодКонтакта= CAST( SUBSTRING (@КодКонтакта, 2, 30) as int)
AND
(T0.От IS NULL OR T0.От <= @Дата)
AND
(T0.До IS NULL OR T0.До > @Дата))
SELECT 1
ELSE
SELECT 0";

        /// <summary>
        ///     Строка запроса: Получаем тип контакта
        /// </summary>
        public const string SELECT_ID_ТипКонтакта = @"
SELECT  КодТипаКонтакта, ТипКонтакта, ТипКонтактаЛат, Категория, icon
FROM    ТипыКонтактов (nolock) 
WHERE   КодТипаКонтакта = @id";

        /// <summary>
        ///     Строка запроса: Получаем типы контакта
        /// </summary>
        public const string SELECT_ТипыКонтактов =
            @"SELECT КодТипаКонтакта, ТипКонтакта, ТипКонтактаЛат FROM ТипыКонтактов";


        /// <summary>
        ///     Строка запроса: редактируем контакт
        /// </summary>
        public const string UPDATE_Контакт_ПоЛицу = @"
UPDATE  vwКонтакты
SET     КодЛица = @КодЛица,
        КодСвязиЛиц = NULL,
        КодТипаКонтакта = @КодТипаКонтакта,
        КодСтраны = @КодСтраны, АдресИндекс = @АдресИндекс, АдресОбласть = @АдресОбласть, АдресГород = @АдресГород, АдресГородRus = @АдресГородRus, Адрес = @Адрес,
        ТелефонСтрана = @ТелефонСтрана, ТелефонГород = @ТелефонГород, ТелефонНомер = @ТелефонНомер,ТелефонДоп = @ТелефонДоп,
        ДругойКонтакт = @ДругойКонтакт,
        Примечание = @Примечание
WHERE   КодКонтакта = @КодКонтакта";

        /// <summary>
        ///     Строка запроса: Редактируем контакт по связи
        /// </summary>
        public const string UPDATE_Контакт_ПоСвязи = @"
UPDATE  vwКонтакты
SET     КодЛица = NULL,
        КодСвязиЛиц = @КодСвязиЛиц,
        КодТипаКонтакта = @КодТипаКонтакта,
        КодСтраны = @КодСтраны, АдресИндекс = @АдресИндекс, АдресОбласть = @АдресОбласть, АдресГород = @АдресГород, АдресГородRus = @АдресГородRus, Адрес = @Адрес,
        ТелефонСтрана = @ТелефонСтрана, ТелефонГород = @ТелефонГород, ТелефонНомер = @ТелефонНомер, ТелефонДоп = @ТелефонДоп,
        ДругойКонтакт = @ДругойКонтакт,
        Примечание = @Примечание
WHERE   КодКонтакта = @КодКонтакта";

        /// <summary>
        ///     Строка запроса: Создаем контакт по связи
        /// </summary>
        public const string INSERT_Контакт_ПоСвязи = @"
INSERT  vwКонтакты(КодСвязиЛиц, КодТипаКонтакта, КодСтраны, АдресИндекс, АдресОбласть, АдресГород, АдресГородRus, Адрес, ТелефонСтрана, ТелефонГород, ТелефонНомер, ТелефонДоп, ДругойКонтакт, Примечание)
SELECT  @КодСвязиЛиц, @КодТипаКонтакта, @КодСтраны, @АдресИндекс, @АдресОбласть, @АдресГород, @АдресГородRus, @Адрес, @ТелефонСтрана,  @ТелефонГород,  @ТелефонНомер, @ТелефонДоп, @ДругойКонтакт, @Примечание
SELECT  SCOPE_IDENTITY() КодКонтакта;";

        /// <summary>
        ///     Строка запроса: Создаем контакт
        /// </summary>
        public const string INSERT_Контакт_ПоЛицу = @"
INSERT  vwКонтакты(КодЛица, КодТипаКонтакта, КодСтраны, АдресИндекс, АдресОбласть, АдресГород, АдресГородRus, Адрес, ТелефонСтрана, ТелефонГород, ТелефонНомер, ТелефонДоп, ДругойКонтакт, Примечание)
SELECT  @КодЛица, @КодТипаКонтакта, @КодСтраны, @АдресИндекс, @АдресОбласть, @АдресГород, @АдресГородRus, @Адрес, @ТелефонСтрана, @ТелефонГород,  @ТелефонНомер, @ТелефонДоп, @ДругойКонтакт, @Примечание
SELECT  SCOPE_IDENTITY() КодКонтакта;";

        /// <summary>
        ///     Строка запроса: Создаем контакт
        /// </summary>
        public const string DELETE_ID_Контакт = @"DELETE FROM vwКонтакты WHERE КодКонтакта = @КодКонтакта";


        /// <summary>
        ///     Строка запроса: Получаем контакт
        /// </summary>
        public const string SELECT_ID_Контакт = @"
SELECT  КодКонтакта, КодЛица, КодСвязиЛиц, КодТипаКонтакта, Контакт, КонтактRL, КодСтраны,
        АдресИндекс, АдресОбласть, АдресГород, АдресГородRus, Адрес,
        ТелефонСтрана, ТелефонГород, ТелефонНомер, ТелефонДоп, ТелефонКорпНомер,
        ДругойКонтакт, Примечание, Изменил, Изменено
FROM    vwКонтакты (nolock) WHERE КодКонтакта = @id";


        /// <summary>
        ///     Строка запроса: Получаем контакты по ID лица
        /// </summary>
        public const string SELECT_Контакты_ПоЛицу = @"
SELECT  КодКонтакта, КодЛица, КодСвязиЛиц, КодТипаКонтакта, Контакт, КонтактRL, КодСтраны,
        АдресИндекс, АдресОбласть, АдресГород, АдресГородRus, Адрес,
        ТелефонСтрана, ТелефонГород, ТелефонНомер, ТелефонДоп, ТелефонКорпНомер,
        ДругойКонтакт, Примечание, Изменил, Изменено
FROM    vwКонтакты (nolock) WHERE КодЛица = @id AND (@КодТипаКонтакта = 0 OR КодТипаКонтакта = @КодТипаКонтакта)";

        /// <summary>
        ///     Формирование контакта FN_Лица_ФормированиеКонтакта
        /// </summary>
        public const string SELECT_FN_Лица_ФормированиеКонтакта = @"
SELECT dbo.fn_Лица_ФормированиеКонтакта(@ТипКонтакта, @АдресИндекс, @АдресОбласть, @АдресГород, @АдресГородRus, @Адрес,@КодСтраны, @ТелефонСтрана,@ТелефонГород ,@ТелефонНомер, @ТелефонДоп, @ДругойКонтакт, 0) Контакт
";

        #endregion

        #region МестаХранения

        /// <summary>
        ///     Строка запроса: Получение мест хранения
        /// </summary>
        public const string SELECT_МестаХранения =
            @"SELECT {0} T0.КодМестаХранения, T0.МестоХранения, T0.Parent, T0.L, T0.R FROM dbo.МестаХранения T0 (nolock)";

        /// <summary>
        ///     Строка запроса: Поиск места хранения по коду
        /// </summary>
        public static string SELECT_ID_МестоХранения =
            string.Format(@"{0} WHERE КодМестаХранения = @id", string.Format(SELECT_МестаХранения, ""));

        /// <summary>
        ///     Строка запроса: Получение подчиненных мест хранения
        /// </summary>
        public static string SELECT_МестоХранения_Подчиненные = string.Format(
            @"{0} WHERE T0.L >= @leftKey AND T0.R <= @rightKey ORDER BY L", string.Format(SELECT_МестаХранения, ""));

        #endregion

        #region Ресурсы/валюта

        /// <summary>
        ///     Строка запроса: Последнее изменение ресурса
        /// </summary>
        public static string SELECT_Ресурс_LastChanged =
            @"SELECT T0.Изменено FROM Ресурсы T0 (nolock) WHERE T0.КодРесурса = @id";

        /// <summary>
        ///     Строка запроса: Получает валюты
        /// </summary>
        public static string SELECT_Валюты = @"
SELECT Валюты.КодВалюты, Ресурсы.РесурсРус AS Название FROM Валюты INNER JOIN Ресурсы ON КодРесурса=КодВалюты";

        /// <summary>
        ///     Строка запроса: Получает валюту по коду
        /// </summary>
        public static string SELECT_ID_Валюты = @"
SELECT Валюты.КодВалюты, Ресурсы.РесурсРус AS Название FROM Валюты INNER JOIN Ресурсы ON КодРесурса=КодВалюты WHERE КодВалюты=@КодВалюты";

        /// <summary>
        ///     Строка запроса: Получает валюту по КодКлассификатораБукв
        /// </summary>
        public static string SELECT_ID_Валюты_ПоКодуКлассификатораБукв = @"
SELECT TOP 1 Валюты.КодВалюты FROM Валюты WHERE КодКлассификатораБукв=@КодКлассификатораБукв";

        /// <summary>
        ///     курс ЦБ РФ
        /// </summary>
        public static string SELECT_LoadKursCbrf = @"
SELECT TOP 1 Курс, Единиц
FROM КурсыВалют
	INNER JOIN Валюты ON КурсыВалют.КодВалюты = Валюты.КодВалюты
WHERE КурсыВалют.КодВалюты = {0} AND ДатаКурса <= '{1}'
	AND ((ТипЦБРФ = 0 AND Инвентаризация.dbo.fn_РабочиеДни(ДатаКурса, '{1}',188) = 0) OR (ТипЦБРФ = 1 AND '{1}' < DATEADD(month, 1, ДатаКурса+1)-1))
ORDER BY ДатаКурса DESC";

        /// <summary>
        ///     Строка запроса: Получение курса валют
        /// </summary>
        public const string SELECT_КурсВалюты =
            @"SELECT ДатаКурса, КодВалюты, Курс, Единиц, Состояние FROM КурсыВалют WHERE КодКурсаВалюты = @id";

        /// <summary>
        ///     Строка запроса: Получение курса выбранных валют за определенный период
        /// </summary>
        public static string SELECT_КурсыВалютЗаПериод = @"
SELECT КурсыВалют.КодКурсаВалюты, КурсыВалют.КодВалюты, Ресурсы.РесурсРус, CASE WHEN LEN(ISNULL(Ресурсы.РесурсЛат,'')) > 0 THEN Ресурсы.РесурсЛат ELSE Ресурсы.РесурсРус END AS РесурсЛат, CONVERT(varchar, КурсыВалют.ДатаКурса, 104) AS ДатаКурса, КурсыВалют.Курс, КурсыВалют.Единиц, КурсыВалют.Состояние, Сотрудники.КодСотрудника, Сотрудники.ФИО, Сотрудники.FIO, КурсыВалют.Изменено
FROM КурсыВалют
	INNER JOIN Ресурсы ON КурсыВалют.КодВалюты = Ресурсы.КодРесурса
	LEFT OUTER JOIN Инвентаризация.dbo.Сотрудники Сотрудники ON Сотрудники.КодСотрудника = КурсыВалют.Изменил
WHERE КурсыВалют.КодВалюты IN (SELECT value FROM Инвентаризация.dbo.fn_SplitInts(@КодыВалют)) AND ДатаКурса >= @ДатаКурсаОт AND ДатаКурса <= @ДатаКурсаПо
ORDER BY ДатаКурса DESC";

        /// <summary>
        ///     Строка запроса: Добавление курса валют
        /// </summary>
        public const string INSERT_КурсВалют = @"
INSERT INTO КурсыВалют (ДатаКурса, КодВалюты, Курс, Единиц, Состояние) 
VALUES (@ДатаКурса, @КодВалюты, @Курс, @Единиц, @Состояние)";

        /// <summary>
        ///     Строка запроса: Обновление курса валют
        /// </summary>
        public const string UPDATE_КурсВалют = @"
UPDATE КурсыВалют SET ДатаКурса = @ДатаКурса, КодВалюты = @КодВалюты, Курс = @Курс, Единиц = @Единиц, Состояние = @Состояние WHERE КодКурсаВалюты = @id";

        /// <summary>
        ///     Строка запроса: Удаление курса валют
        /// </summary>
        public const string DELETE_КурсВалют = @"DELETE FROM КурсыВалют WHERE КодКурсаВалюты = @id";

        /// <summary>
        ///     Строка запроса: Получение среднего курса выбранных валют за определенный период
        /// </summary>
        public static string SELECT_СредниеКурсыВалютЗаПериод = @"
SET NOCOUNT ON 
IF object_id('tempdb..#Курсы') IS NOT NULL DROP TABLE #Курсы 
CREATE TABLE #Курсы(Код int PRIMARY KEY IDENTITY(1,1), КодВалюты int, ДатаКурса smalldatetime, Курс money) 
DECLARE @Валюты TABLE(КодВалюты int) 
DECLARE @КодВалюты int, @ТекущаяДата smalldatetime, @ТекущаяДатаS varchar(20)
INSERT @Валюты 
SELECT value FROM Инвентаризация.dbo.fn_SplitInts(@КодыВалют)   
INSERT #Курсы
SELECT КодВалюты, ДатаКурса, Курс
FROM КурсыВалют
WHERE	КодВалюты IN (SELECT КодВалюты FROM @Валюты X)
	AND ДатаКурса BETWEEN @ДатаКурсаОт AND @ДатаКурсаПо
ORDER BY КодВалюты, ДатаКурса
WHILE EXISTS(SELECT * FROM @Валюты) 
BEGIN 
    SET @ТекущаяДата = @ДатаКурсаОт 
    SELECT TOP 1 @КодВалюты = КодВалюты FROM @Валюты   
    WHILE @ТекущаяДата <= @ДатаКурсаПо 
    BEGIN
	    IF NOT EXISTS(SELECT * FROM #Курсы WHERE КодВалюты = @КодВалюты AND ДатаКурса = @ТекущаяДата)
	    BEGIN
		    INSERT #Курсы
		    SELECT TOP 1 КодВалюты, @ТекущаяДата, Курс FROM КурсыВалют WHERE КодВалюты = @КодВалюты AND ДатаКурса < @ТекущаяДата ORDER BY КурсыВалют.ДатаКурса DESC
	    END
	    SET @ТекущаяДата = DATEADD(d, 1, @ТекущаяДата) 
    END 
    DELETE @Валюты WHERE КодВалюты = @КодВалюты 
END
SELECT КодВалюты, Ресурсы.РесурсРус, CASE WHEN LEN(ISNULL(Ресурсы.РесурсЛат,'')) > 0 THEN Ресурсы.РесурсЛат ELSE Ресурсы.РесурсРус END AS РесурсЛат, CASE WHEN DATEDIFF(day, @ДатаКурсаОт , @ТекущаяДата) = 0 THEN 0 ELSE SUM(Курс)/DATEDIFF(day, @ДатаКурсаОт, @ТекущаяДата) END Курс 
FROM #Курсы X INNER JOIN Ресурсы ON X.КодВалюты = Ресурсы.КодРесурса 
GROUP BY КодВалюты, Ресурсы.РесурсРус, Ресурсы.РесурсЛат
ORDER BY Ресурсы.РесурсРус   
DROP TABLE #Курсы";

        /// <summary>
        ///     Строка запроса: Получение среднего курса выбранных валют за определенный период
        /// </summary>
        public static string SELECT_КроссКурсВалютЗаПериод = @"
;WITH EnumSel AS
(
	SELECT t1.ДатаКурса AS Дата, 
		t1.Курс/t2.Курс AS Курс, 
		ROW_NUMBER() OVER (ORDER BY t1.ДатаКурса DESC) AS Row#
	FROM КурсыВалют AS t1 
		INNER JOIN КурсыВалют AS t2 ON t1.ДатаКурса = t2.ДатаКурса
	WHERE t1.КодВалюты = @КодВалютыИсточник 
		AND t2.КодВалюты = @КодВалютыЦель 
		AND t1.ДатаКурса >= DATEADD(d, -14, @ДатаКурсаОт) 
		AND t1.ДатаКурса <= @ДатаКурсаПо
)
SELECT CurRows.Дата, CurRows.Курс,
CASE WHEN PrevRows.Курс IS NOT NULL OR PrevRows.Курс > 0 THEN (CurRows.Курс-PrevRows.Курс)*100/PrevRows.Курс ELSE 0 END AS Изменение
FROM EnumSel AS CurRows
	LEFT JOIN EnumSel AS PrevRows ON CurRows.Row# = PrevRows.Row# - 1
WHERE CurRows.Дата >= @ДатаКурсаОт";

        /// <summary>
        ///     Строка запроса: Получение ресурсов
        /// </summary>
        public const string SELECT_Ресурсы = @"
SELECT {0}  T0.КодРесурса, ISNULL(NULLIF(T0.РесурсРус,''),'#' + CONVERT(varchar,T0.КодРесурса)) РесурсРус, T0.РесурсЛат, T0.РесурсRL,
            T0.КодЕдиницыИзмерения, T0.КодВидаПодакцизногоТовара, T0.Точность, T0.СпецНДС, T0.Parent, T0.L, T0.R, T0.Изменено
FROM    Ресурсы T0 (nolock)";

        /// <summary>
        ///     Строка запроса: Поиск ресурса по коду
        /// </summary>
        public static string SELECT_ID_Ресурс =
            string.Format(@"{0} WHERE T0.КодРесурса = @id", string.Format(SELECT_Ресурсы, ""));

        /// <summary>
        ///     Строка запроса: Получает все валюты
        /// </summary>
//         public static string SELECT_Ресурсы_Валюты = @"
//SELECT  child.КодРесурса, child.РесурсРус, u.ЕдиницаРус, u.ЕдиницаЛат, child.Точность 
//FROM    Ресурсы parent (nolock) INNER JOIN 
//        Ресурсы child (nolock) ON parent.L < child.L AND parent.R > child.R LEFT JOIN 
//        ЕдиницыИзмерения u (nolock) ON child.КодЕдиницыИзмерения = u.КодЕдиницыИзмерения WHERE child.Parent = 1";
        public static string SELECT_Ресурсы_Валюты = @"
SELECT Ресурсы.КодРесурса, Ресурсы.РесурсРус, u.ЕдиницаРус, u.ЕдиницаЛат, Ресурсы.Точность,
	Валюты.КодКлассификатораЦифр AS OKV_ID_Digital, Валюты.КодКлассификатораБукв AS OKV_ID_Literal, Ресурсы.РесурсЛат
FROM Валюты
	INNER JOIN Ресурсы ON Валюты.КодВалюты = Ресурсы.КодРесурса
	INNER JOIN ЕдиницыИзмерения u ON Ресурсы.КодЕдиницыИзмерения = u.КодЕдиницыИзмерения
ORDER BY Ресурсы.L";

        /// <summary>
        ///     Строка запроса: Получение подчиненных ресурсов
        /// </summary>
        public static string SELECT_Ресурс_Подчиненные =
            string.Format(@"{0} WHERE T0.L >= @leftKey AND T0.R <= @rightKey ORDER BY T0.L",
                string.Format(SELECT_Ресурсы, ""));

        /// <summary>
        ///     Строка запроса: Получение единицы измерения
        /// </summary>
        public const string SELECT_ID_ЕдиницаИзмерения =
            @"SELECT * FROM ЕдиницыИзмерения WHERE КодЕдиницыИзмерения=@id";

        #endregion

        #region Склады

        /// <summary>
        ///     Хранимая процедура: Поиск склада
        /// </summary>
        public const string SP_Склады_Поиск_NEW = "sp_Склады_Поиск_NEW";

        /// <summary>
        ///     Хранимая процедура: Сохранение склада
        /// </summary>
        public const string SP_Лица_InsUpd_Склады = "sp_Лица_InsUpd_Склады";

        /// <summary>
        ///     Строка запроса: Типы складов
        /// </summary>
        public const string SELECT_ТипыСкладов =
            @"SELECT {0} T0.КодТипаСклада, T0.ТипСклада, T0.Псевдоним, T0.КорневойРесурс, T0.Примечание FROM ТипыСкладов T0 (nolock)";

        /// <summary>
        ///     Строка запроса: Поиск типа склада по коду
        /// </summary>
        public static string SELECT_ID_ТипСклада =
            string.Format(@"{0} WHERE T0.КодТипаСклада = @id", string.Format(SELECT_ТипыСкладов, ""));

        /// <summary>
        ///     Строка запроса: Поиск склада
        /// </summary>
        public const string SELECT_Склад = @"
SELECT {0} КодСклада, Склад, COALESCE(NULLIF(Склад, '') + '/' + NULLIF(IBAN, ''), NULLIF(Склад, ''), NULLIF(IBAN, ''), '_____________________') AS Название
FROM    vwСклады T0 (nolock)";

        /// <summary>
        ///     Строка запроса: Поиск склада
        /// </summary>
        public const string SELECT_Склад_Ext = @"
SELECT {0} * FROM (SELECT [T10].[КодСклада], [T10].[Склад], [T10].[IBAN], [T10].[КодТипаСклада], [T10].[КодМестаХранения], [T10].[КодРесурса],  
        [T10].[КодХранителя],  [T10].[КодРаспорядителя],  [T10].[КодПодразделенияРаспорядителя], [T10].[КодДоговора], [T10].[Филиал], [T10].[Примечание], [T10].[Изменил], 
        [T10].[Изменено], ТС.Псевдоним, Ресурсы.РесурсРус Ресурс,
        Склад + CASE WHEN Склад <> '' AND IBAN <> '' THEN '/' ELSE '' END + IBAN  СкладFull, Склад Sort, Хранители.Кличка Хранитель, Распорядители.Кличка Распорядитель,
        [T10].От,
        [T10].До
FROM vwСклады T10 (nolock)
INNER JOIN ТипыСкладов ТС ON ТС.КодТипаСклада = T10.КодТипаСклада
INNER JOIN Ресурсы (nolock) ON Ресурсы.КодРесурса = T10.КодРесурса
LEFT JOIN vwЛица Хранители (nolock) ON Хранители.КодЛица = T10.КодХранителя LEFT JOIN vwЛица Распорядители (nolock) ON Распорядители.КодЛица = T10.КодРаспорядителя) T0 
";


        /// <summary>
        ///     Строка запроса: Последнее изменение склада
        /// </summary>
        public const string SELECT_Склад_LastChanged =
            @"SELECT T0.Изменено FROM vwСклады T0 (nolock) WHERE КодСклада=@id";

        /// <summary>
        ///     Строка запроса: Поиск склада по ID
        /// </summary>
        public const string SELECT_ID_Склад = @"
SELECT {0} КодСклада, Склад, COALESCE( NULLIF(Склад, '') + '/' + NULLIF(IBAN, ''), NULLIF(Склад, ''), NULLIF(IBAN, ''), '_____________________') AS Название
FROM    vwСклады T0 (nolock) WHERE КодСклада=@КодСклада";

        /// <summary>
        ///     Строка запроса: Поиск склада по ID
        /// </summary>
        public const string SELECT_ID_СкладПодробно = @"
SELECT {0} КодСклада, Склад, COALESCE( NULLIF(Склад, '') + '/' + NULLIF(IBAN, ''), NULLIF(Склад, ''), NULLIF(IBAN, ''), '_____________________') AS Название,
IBAN, КодТипаСклада, КодМестаХранения, КодРесурса, КодХранителя, КодРаспорядителя, КодПодразделенияРаспорядителя, КодДоговора, Филиал, Примечание, От, До, Изменено, Изменил
FROM    vwСклады T0 (nolock) WHERE КодСклада=@КодСклада";

        /// <summary>
        ///     Лица склада склада действуют на дату
        /// </summary>
        public const string SELECT_TEST_ЛицаСкладаДействуют = @"
IF (@КодХранителя IS NULL OR EXISTS( SELECT NULL FROM fn_ДействующиеЛица(@Дата) WHERE @КодХранителя=КодЛица))
AND (@КодРаспорядителя IS NULL OR EXISTS( SELECT NULL FROM fn_ДействующиеЛица(@Дата) WHERE @КодРаспорядителя=КодЛица))
SELECT 1
ELSE
SELECT 0";

        /// <summary>
        ///     Строка запроса: Поиск параметров склада по ID
        /// </summary>
        public const string SELECT_СкладПараметры = @"
SELECT КодСклада, От , DATEADD(day, -1, До) AS По, Склад, IBAN, КодТипаСклада, КодМестаХранения, КодРесурса,
КодХранителя, КодРаспорядителя, КодПодразделенияРаспорядителя, КодДоговора, Филиал, Примечание, Изменил, Изменено
FROM vwСклады WHERE КодСклада=@КодСклада";

        /// <summary>
        ///     Строка запроса: Список банковских счетов компании
        /// </summary>
        public const string SELECT_БанковскиеСчетаКомпании = @"
SELECT	КодСклада, ТипыСкладов.ТипСклада Тип, Склад + CASE WHEN Склад <>'' AND IBAN <> '' THEN '/' ELSE '' END + IBAN Счет, ЕдиницыИзмерения.ЕдиницаРус Валюта,
    vwСклады.КодХранителя КодБанка,
	ISNULL(Банки.Кличка, '#' + CONVERT(varchar, vwСклады.КодХранителя)) Банк
FROM	vwСклады INNER JOIN 
	ТипыСкладов ON vwСклады.КодТипаСклада = ТипыСкладов.КодТипаСклада INNER JOIN
	Ресурсы ON vwСклады.КодРесурса = Ресурсы.КодРесурса INNER JOIN 
	ЕдиницыИзмерения ON Ресурсы.КодЕдиницыИзмерения = ЕдиницыИзмерения.КодЕдиницыИзмерения LEFT JOIN
	vwЛица Банки ON vwСклады.КодХранителя = Банки.КодЛица
WHERE	КодРаспорядителя = @КодРаспорядителя 
	AND ISNULL(От, '19800101') <= @Дата AND @Дата < ISNULL(До, '20500101')
	AND vwСклады.КодТипаСклада > 0 AND vwСклады.КодТипаСклада < 6
ORDER BY vwСклады.КодТипаСклада, Банк
";

        /// <summary>
        ///     Строка запроса: Определяет сколько разных ресурсов в переданном списке складов
        /// </summary>
        public const string SELECT_КоличествоВалютВСчетахКомпании = @"
SELECT  COUNT(DISTINCT КодРесурса) N 
FROM	vwСклады 
WHERE	КодСклада IN(SELECT value FROM Инвентаризация.dbo.fn_SplitInts(@accountIds))
        AND ISNULL(От, '19800101') <= @Дата AND @Дата < ISNULL(До, '20500101') 
        AND (vwСклады.КодХранителя IS NULL OR EXISTS( SELECT NULL FROM dbo.fn_ДействующиеЛица(@Дата) X WHERE vwСклады.КодХранителя=X.КодЛица))
        AND (vwСклады.КодРаспорядителя IS NULL OR EXISTS( SELECT NULL FROM dbo.fn_ДействующиеЛица(@Дата) X WHERE vwСклады.КодРаспорядителя=X.КодЛица))
";

        /// <summary>
        ///     Строка запроса: Определяет сколько разных ресурсов в переданном списке складов
        /// </summary>
        public const string SELECT_РаспорядительИмеетСкладыУказанныхТипов = @"
IF EXISTS(
SELECT * 
FROM    vwСклады
WHERE   ISNULL(От, '19800101') <= @Дата AND @Дата < ISNULL(До, '20500101') 
        AND КодТипаСклада IN(SELECT value FROM Инвентаризация.dbo.fn_SplitInts(@ТипыСкладов))
        AND КодРаспорядителя = @КодРаспорядителя
        AND (vwСклады.КодХранителя IS NULL OR EXISTS( SELECT NULL FROM dbo.fn_ДействующиеЛица(@Дата) X WHERE vwСклады.КодХранителя=X.КодЛица))
        AND (vwСклады.КодРаспорядителя IS NULL OR EXISTS( SELECT NULL FROM dbo.fn_ДействующиеЛица(@Дата) X WHERE vwСклады.КодРаспорядителя=X.КодЛица))
)
    SELECT 1
ELSE
    SELECT 0
";


        /// <summary>
        ///     Строка запроса: Возвращает список ресурсов складов компании
        /// </summary>
        public const string SELECT_ВалютыВСчетахКомпании = @"
SELECT  DISTINCT КодРесурса
FROM	vwСклады 
WHERE	ISNULL(От, '19800101') <= @Дата AND @Дата < ISNULL(До, '20500101') 
        AND КодТипаСклада IN(SELECT value FROM Инвентаризация.dbo.fn_SplitInts(@ТипыСкладов))
        AND КодРаспорядителя = @КодРаспорядителя
        AND (vwСклады.КодХранителя IS NULL OR EXISTS( SELECT NULL FROM dbo.fn_ДействующиеЛица(@Дата) X WHERE vwСклады.КодХранителя=X.КодЛица))
        AND (vwСклады.КодРаспорядителя IS NULL OR EXISTS( SELECT NULL FROM dbo.fn_ДействующиеЛица(@Дата) X WHERE vwСклады.КодРаспорядителя=X.КодЛица))
";

        /// <summary>
        ///     Строка запроса: Поиск складов по названию
        /// </summary>
        public const string SELECT_ПоискСкладовПоНазванию = @"
SET @searchText = RTRIM(LTRIM(Инвентаризация.dbo.fn_ReplaceKeySymbols(Инвентаризация.dbo.fn_SplitWords(@searchText))))
WHILE CHARINDEX('  ',@searchText) > 0 SET @searchText = REPLACE(@searchText,'  ',' ')

SET @searchText='%' + REPLACE(@searchText,' ','% ') + '%'
SELECT КодСклада
FROM Справочники.dbo.vwСклады Склады
	LEFT OUTER JOIN Справочники.dbo.vwЛица Хранители ON Хранители.КодЛица = Склады.КодХранителя
WHERE (Склад +' ' + IBAN + ' ' +Хранители.Кличка) LIKE @searchText
";

        #endregion

        #region Транспорт

        /// <summary>
        ///     Строка запроса: Получение транспортных узлов
        /// </summary>
        public const string SELECT_ТранспортныеУзлы = @"SELECT {0} * FROM dbo.ТранспортныеУзлы T0 (nolock)";

        /// <summary>
        ///     Строка запроса: Транспортные узлы по коду
        /// </summary>
        public static string SELECT_ID_ТранспортныеУзлы = string.Format(@"{0} WHERE КодТранспортногоУзла = @id",
            string.Format(SELECT_ТранспортныеУзлы, ""));

        /// <summary>
        ///     Строка запроса: Транспортный узел по коду
        /// </summary>
        public static string SELECT_ID_ТранспортныйУзел = @"
SELECT TOP 1 T0.КодТранспортногоУзла, T0.Название, T0.Название+ISNULL(' ['+ЖД.ЖелезнаяДорога +']','') НазваниеЖД, 
            ВТ.ВидТранспорта Транспорт, T0.КодЖелезнойДороги,T0.КодВидаТранспорта		
FROM [Справочники].[dbo].ТранспортныеУзлы T0
LEFT OUTER JOIN [Справочники].[dbo].ЖелезныеДороги ЖД ON T0.КодЖелезнойДороги=ЖД.КодЖелезнойДороги
INNER JOIN [Справочники].[dbo].ВидыТранспорта ВТ ON  T0.КодВидаТранспорта=ВТ.КодВидаТранспорта	
WHERE T0.КодТранспортногоУзла= {0}
";

        /// <summary>
        ///     Строка запроса: Список для выбора транспортных узлов
        /// </summary>
        public static string SELECT_ТранспортныеУзлыВыбор(string search, string type, int maxItemsInQuery)
        {
            return string.Format(
                @"SELECT TOP {0} T0.КодТранспортногоУзла, T0.Название+ISNULL('['+ЖД.ЖелезнаяДорога +']','') Название, ВТ.ВидТранспорта Транспорт, T0.КодЖелезнойДороги,T0.КодВидаТранспорта		
FROM [Справочники].[dbo].ТранспортныеУзлы T0
LEFT OUTER JOIN [Справочники].[dbo].ЖелезныеДороги ЖД ON T0.КодЖелезнойДороги=ЖД.КодЖелезнойДороги
INNER JOIN [Справочники].[dbo].ВидыТранспорта ВТ ON  T0.КодВидаТранспорта=ВТ.КодВидаТранспорта	
WHERE (1=1) {1} {2} ORDER BY T0.Название,T0.КодВидаТранспорта", maxItemsInQuery,
                !string.IsNullOrEmpty(type) ? @" and T0.КодВидаТранспорта = " + type : "",
                !string.IsNullOrEmpty(search)
                    ? @" and (' ' + (CASE WHEN T0.КодВидаТранспорта = 1 THEN RIGHT((T0.КодТранспортногоУзла+1000000),6) ELSE CONVERT(varchar,T0.КодТранспортногоУзла) END) + ' ' +
T0.НазваниеRL LIKE N'% ' + Инвентаризация.dbo.fn_ReplaceRusLat(N'" + search + "') + '%')"
                    : "");
        }

        #endregion

        #region Единицы измерения

        /// <summary>
        ///     Строка запроса: Получить Дополнительные Единицы измерения
        /// </summary>
        public const string SELECT_ЕдиницыИзмеренияДоп = @"SELECT 
	            КодЕдиницыИзмеренияДополнительной, КодРесурса, КодЕдиницыИзмерения, Точность, КоличествоЕдиниц, КоличествоЕдиницОсновных, Коэффициент, МассаБрутто
                FROM ЕдиницыИзмеренияДополнительные WHERE КодРесурса = @КодРесурса";

        /// <summary>
        ///     Строка запроса: Получить Дополнительные Единицы измерения
        /// </summary>
        public const string SELECT_ID_ЕдиницаИзмеренияДополнительные = @"SELECT 
	            КодЕдиницыИзмеренияДополнительной, КодРесурса, КодЕдиницыИзмерения, Точность, КоличествоЕдиниц, КоличествоЕдиницОсновных, Коэффициент, МассаБрутто
                FROM ЕдиницыИзмеренияДополнительные WHERE КодЕдиницыИзмеренияДополнительной = @id";

        /// <summary>
        ///     Строка запроса: Получить Единицы измерения
        /// </summary>
        public const string SELECT_ЕдиницыИзмерения = @"SELECT КодЕдиницыИзмерения, Описание FROM ЕдиницыИзмерения";

        /// <summary>
        ///     Строка запроса: Получить Единицы измерения дополнительные для одного ресурса
        /// </summary>
        public const string SELECT_ЕдиницыИзмеренияДополнительные = @"
        SELECT * FROM (
	        SELECT 10000001 КодЕдиницыИзмеренияДополнительной, Ресурсы.КодРесурса, ЕдиницыИзмерения.КодЕдиницыИзмерения,
		        Описание, ЕдиницаРус, ЕдиницаЛат, 1 Коэффициент, '' ЕдиницаРусОсн, '' ЕдиницаЛатОсн, 1 Порядок
	        FROM Ресурсы
		        INNER JOIN ЕдиницыИзмерения ON Ресурсы.КодЕдиницыИзмерения=ЕдиницыИзмерения.КодЕдиницыИзмерения
            WHERE Ресурсы.КодРесурса = @КодРесурса
	        UNION
	        SELECT КодЕдиницыИзмеренияДополнительной, Ресурсы.КодРесурса, Доп.КодЕдиницыИзмерения,
		        Доп.Описание, Доп.ЕдиницаРус, Доп.ЕдиницаЛат, ЕдиницыИзмеренияДополнительные.Коэффициент, Осн.ЕдиницаРус ЕдиницаРусОсн, Осн.ЕдиницаЛат ЕдиницаЛатОсн , 2 Порядок
	        FROM ЕдиницыИзмеренияДополнительные
		        INNER JOIN ЕдиницыИзмерения Доп ON Доп.КодЕдиницыИзмерения=ЕдиницыИзмеренияДополнительные.КодЕдиницыИзмерения
		        INNER JOIN Ресурсы ON ЕдиницыИзмеренияДополнительные.КодРесурса=Ресурсы.КодРесурса
		        INNER JOIN ЕдиницыИзмерения Осн ON Ресурсы.КодЕдиницыИзмерения=Осн.КодЕдиницыИзмерения
            WHERE Ресурсы.КодРесурса = @КодРесурса
        ) T0";

        /// <summary>
        ///     Строка запроса: Получить Единицы измерения дополнительные для нескольких ресурсов
        /// </summary>
        public const string SELECT_ЕдиницыИзмеренияДополнительныеНесколькоРесурсов = @"
        SELECT * FROM (SELECT 10000001 КодЕдиницыИзмеренияДополнительной, -1 КодРесурса, -1 КодЕдиницыИзмерения, '' Описание, '' ЕдиницаРус, '' ЕдиницаЛат, 1 Коэффициент, 1 Порядок
        UNION
        SELECT КодЕдиницыИзмеренияДополнительной, КодРесурса, Доп.КодЕдиницыИзмерения, Доп.Описание, Доп.ЕдиницаРус, Доп.ЕдиницаЛат, ЕдиницыИзмеренияДополнительные.Коэффициент , 2 Порядок
        FROM ЕдиницыИзмеренияДополнительные
	        INNER JOIN ЕдиницыИзмерения Доп ON Доп.КодЕдиницыИзмерения=ЕдиницыИзмеренияДополнительные.КодЕдиницыИзмерения
        ) T0";

        #endregion

        #region Статьи Движения Денежных Средств

        /// <summary>
        ///     Получение информации для дерева
        /// </summary>
        public static string SELECT_СтатьиДвиженияДенежныхСредствДанныеДляДерева = @"
SELECT  r.КодСтатьиДвиженияДенежныхСредств Id, r.R-L ЕстьДети, r.СтатьяДвиженияДенежныхСредств Text, ISNULL(r.Parent,0) ParentId, '' Фильтр, ISNULL(rr.ВидДвиженияДенежныхСредств,0) ВидДвиженияДенежныхСредств
FROM    СтатьиДвиженияДенежныхСредств r 
LEFT JOIN  ВидыДвиженийДенежныхСредств rr ON r.КодВидаДвиженияДенежныхСредств = rr.КодВидаДвиженияДенежныхСредств
WHERE   ((@Потомки = 1 AND ((@Код = 0 AND r.Parent IS NULL) OR (@Код <> 0 AND r.Parent = @Код)))
        OR (@Потомки = 0 AND r.КодСтатьиДвиженияДенежныхСредств = @Код)) 
ORDER BY r.{0}
";

        /// <summary>
        ///     Получение информации для дерева
        /// </summary>
        public static string SELECT_СтатьиДвиженияДенежныхСредствДанныеДляДерева_State = @"
SET NOCOUNT ON

IF OBJECT_ID('tempdb.#Статьи') IS NOT NULL DROP TABLE #Статьи
CREATE TABLE #Статьи(
        КодЗаписи int IDENTITY(1,1),
        [КодСтатьиДвиженияДенежныхСредств] [int],
        СтатьяДвиженияДенежныхСредств [varchar](300),       
        [Parent] [int],
        [L] [int],
        [R] [int],
        [Изменил] [int],
        [Изменено] [datetime],  
        BitMask tinyint
)

INSERT #Статьи
SELECT	Parent.[КодСтатьиДвиженияДенежныхСредств],
        Parent.СтатьяДвиженияДенежныхСредств,      
        Parent.[Parent],
        Parent.[L],
        Parent.[R],
        Parent.[Изменил],
        Parent.[Изменено], 
        1 BitMask
FROM	СтатьиДвиженияДенежныхСредств Parent 
WHERE EXISTS(SELECT * FROM СтатьиДвиженияДенежныхСредств Child 
					WHERE	Child.КодСтатьиДвиженияДенежныхСредств IN ({1})
						AND Parent.L <=	Child.L AND Parent.R>=Child.R) 
      AND NOT EXISTS(SELECT * FROM #Статьи X WHERE Parent.КодСтатьиДвиженияДенежныхСредств = X.КодСтатьиДвиженияДенежныхСредств)
ORDER BY Parent.L

INSERT #Статьи
SELECT	Child.[КодСтатьиДвиженияДенежныхСредств],
        Child.СтатьяДвиженияДенежныхСредств,      
        Child.[Parent],
        Child.[L],
        Child.[R],
        Child.[Изменил],
        Child.[Изменено], 
        2 BitMask
FROM	СтатьиДвиженияДенежныхСредств Parent
LEFT JOIN СтатьиДвиженияДенежныхСредств Child ON Child.Parent = Parent.КодСтатьиДвиженияДенежныхСредств
WHERE Parent.КодСтатьиДвиженияДенежныхСредств IN ({1}) AND NOT EXISTS(SELECT * FROM #Статьи X WHERE Child.КодСтатьиДвиженияДенежныхСредств = X.КодСтатьиДвиженияДенежныхСредств)
ORDER BY Parent.L

INSERT #Статьи 
SELECT  [КодСтатьиДвиженияДенежныхСредств],
        СтатьяДвиженияДенежныхСредств,      
        [Parent],
        [L],
        [R],
        [Изменил],
        [Изменено], 
        4 BitMask
FROM    СтатьиДвиженияДенежныхСредств 
WHERE   Parent IS NULL                                  
        AND NOT EXISTS(SELECT * FROM #Статьи X WHERE СтатьиДвиженияДенежныхСредств.КодСтатьиДвиженияДенежныхСредств = X.КодСтатьиДвиженияДенежныхСредств) 

SELECT #Статьи.[КодСтатьиДвиженияДенежныхСредств] id,
       #Статьи.СтатьяДвиженияДенежныхСредств text,      
       #Статьи.[Parent] ParentId,
       #Статьи.[L],
       #Статьи.[R],
       #Статьи.[Изменил],
       #Статьи.[Изменено], 
       #Статьи.BitMask,
       #Статьи.R-#Статьи.L ЕстьДети
FROM #Статьи
ORDER BY {0}
DROP TABLE #Статьи
";

        /// <summary>
        ///     Строка запроса: Фильтрация статей движения денежных средств
        /// </summary>
        public static string SELECT_СтатьиДвиженияДенежныхСредствДанныеДляДерева_Фильтр = @"
DECLARE @МаксимальноеКоличествоНайденных int = 100
SET NOCOUNT ON
               
IF OBJECT_ID('tempdb..#Статьи') IS NOT NULL DROP TABLE #Статьи
--DECLARE @КоличествоНайденных int
CREATE TABLE #Статьи(
        КодЗаписи int IDENTITY(1,1),
        [КодСтатьиДвиженияДенежныхСредств] [int],
        СтатьяДвиженияДенежныхСредств [varchar](300),       
        [Parent] [int],
        [L] [int],
        [R] [int],
        [Изменил] [int],
        [Изменено] [datetime],  
        BitMask tinyint          
)

INSERT #Статьи
SELECT  [КодСтатьиДвиженияДенежныхСредств],
        СтатьяДвиженияДенежныхСредств,      
        [Parent],
        [L],
        [R],
        [Изменил],
        [Изменено], 
        1 BitMask
FROM    СтатьиДвиженияДенежныхСредств 
WHERE   СтатьяДвиженияДенежныхСредств LIKE '{1}'
ORDER BY L
 
SET @КоличествоНайденных = @@ROWCOUNT
DELETE #Статьи WHERE КодЗаписи > @МаксимальноеКоличествоНайденных
 
UPDATE  Parent
SET     BitMask = BitMask ^ 2
FROM    #Статьи Parent
WHERE   EXISTS(SELECT * FROM #Статьи Child WHERE Parent.L < Child.L AND Parent.R > Child.R)
 
INSERT  #Статьи
SELECT  [КодСтатьиДвиженияДенежныхСредств],
        СтатьяДвиженияДенежныхСредств,      
        [Parent],
        [L],
        [R],
        [Изменил],
        [Изменено], 
        2 BitMask
FROM    СтатьиДвиженияДенежныхСредств Parent 
WHERE   EXISTS( SELECT * FROM #Статьи Child 
                WHERE Parent.L <= Child.L AND Parent.R>=Child.R)                                        
        AND NOT EXISTS(SELECT * FROM #Статьи X WHERE Parent.КодСтатьиДвиженияДенежныхСредств = X.КодСтатьиДвиженияДенежныхСредств)
 
UPDATE  #Статьи
SET     BitMask = BitMask ^ 4
WHERE   Parent IS NULL
 
INSERT #Статьи 
SELECT  [КодСтатьиДвиженияДенежныхСредств],
        СтатьяДвиженияДенежныхСредств,      
        [Parent],
        [L],
        [R],
        [Изменил],
        [Изменено], 
        4 BitMask
FROM    СтатьиДвиженияДенежныхСредств 
WHERE   Parent IS NULL                                  
        AND NOT EXISTS(SELECT * FROM #Статьи X WHERE СтатьиДвиженияДенежныхСредств.КодСтатьиДвиженияДенежныхСредств = X.КодСтатьиДвиженияДенежныхСредств)
        
SELECT [КодСтатьиДвиженияДенежныхСредств] id,
        СтатьяДвиженияДенежныхСредств text,      
        [Parent] ParentId,
        [L],
        [R],
        [Изменил],
        [Изменено], 
        BitMask,
        R-L ЕстьДети
FROM #Статьи
ORDER BY {0}
DROP TABLE #Статьи
";

        /// <summary>
        ///     Получение детальной информации
        /// </summary>
        public static string SELECT_СтатьяДвиженияДенежныхСредств = @"
SELECT  T0.КодСтатьиДвиженияДенежныхСредств Id, T0.R-L ЕстьДети, T0.СтатьяДвиженияДенежныхСредств Text, ISNULL(T0.Parent,0) ParentId, ISNULL(T1.ВидДвиженияДенежныхСредств,0) ВидДвиженияДенежныхСредств
FROM    СтатьиДвиженияДенежныхСредств T0
LEFT JOIN  ВидыДвиженийДенежныхСредств T1 ON T0.КодВидаДвиженияДенежныхСредств = T1.КодВидаДвиженияДенежныхСредств
WHERE T0.КодСтатьиДвиженияДенежныхСредств = @Код 
";


        /// <summary>
        ///     Строка запроса: Получение денежных средств
        /// </summary>
        public const string SELECT_ДенежныеСредства = @"
SELECT {0} T0.КодСтатьиДвиженияДенежныхСредств, T0.СтатьяДвиженияДенежныхСредств, T0.КодВидаДвиженияДенежныхСредств, T0.Parent, T0.L, T0.R
FROM dbo.СтатьиДвиженияДенежныхСредств T0 (nolock)";

        /// <summary>
        ///     Строка запроса: Поиск денежных средств по коду
        /// </summary>
        public static string SELECT_ДенежныеСредстваПоID = @"
SELECT T0.КодСтатьиДвиженияДенежныхСредств, T0.СтатьяДвиженияДенежныхСредств, T0.КодВидаДвиженияДенежныхСредств, T0.Parent, T0.L, T0.R, ISNULL(T1.ВидДвиженияДенежныхСредств,'') ВидДвиженияДенежныхСредств,
dbo.fn_Tree_СтатьиДвиженияДенежныхСредств_FullPath(T0.КодСтатьиДвиженияДенежныхСредств,1) FullPath, Изменил, Изменено
FROM СтатьиДвиженияДенежныхСредств T0
LEFT JOIN  ВидыДвиженийДенежныхСредств T1 ON T0.КодВидаДвиженияДенежныхСредств = T1.КодВидаДвиженияДенежныхСредств
WHERE T0.КодСтатьиДвиженияДенежныхСредств = @Id ";

        /// <summary>
        ///     Получение информации видов движений денежных средств
        /// </summary>
        public static string SELECT_ВидыДвиженийДенежныхСредств = @"
SELECT  КодВидаДвиженияДенежныхСредств, ВидДвиженияДенежныхСредств, Название1С
FROM    ВидыДвиженийДенежныхСредств 
";

        /// <summary>
        ///     Строка запроса:  Получение информации видов движений денежных средств, отсортированных по
        ///     КодВидаДвиженияДенежныхСредств
        /// </summary>
        public static string SELECT_ВидыДвиженийДенежныхСредств_ID =
            SELECT_ВидыДвиженийДенежныхСредств + " ORDER BY КодВидаДвиженияДенежныхСредств";

        /// <summary>
        ///     Добавить вид движения денежных средств и получить его идентификатор
        /// </summary>
        public const string INSERT_ВидыДвиженияДенежныхСредств = @"
INSERT ВидыДвиженийДенежныхСредств (КодВидаДвиженияДенежныхСредств, ВидДвиженияДенежныхСредств, Название1С) VALUES (@КодВидаДвиженияДенежныхСредств, @ВидДвиженияДенежныхСредств, @Название1С)
SELECT SCOPE_IDENTITY()";

        /// <summary>
        ///     Обновление вид движения денежных средств
        /// </summary>
        public const string UPDATE_ВидыДвиженияДенежныхСредств = @"
UPDATE ВидыДвиженийДенежныхСредств SET ВидДвиженияДенежныхСредств=@ВидДвиженияДенежныхСредств, Название1С=@Название1С WHERE КодВидаДвиженияДенежныхСредств=@КодВидаДвиженияДенежныхСредств
";

        /// <summary>
        ///     Строка запроса: Получение вида движения денежных средств
        /// </summary>
        public const string SELECT_ID_ВидДвиженияДенежныхСредств =
            @"SELECT * FROM ВидыДвиженийДенежныхСредств WHERE КодВидаДвиженияДенежныхСредств=@id";

        /// <summary>
        ///     Строка запроса: Удаление вида движения денежных средств
        /// </summary>
        public const string Delete_ВидыДвиженияДенежныхСредств =
            @"DELETE FROM ВидыДвиженийДенежныхСредств WHERE КодВидаДвиженияДенежныхСредств=@КодВидаДвиженияДенежныхСредств";

        /// <summary>
        ///     Строка запроса: Удаление статьи движения денежных средств
        /// </summary>
        public const string Delete_СтатьиДвиженияДенежныхСредств =
            @"DELETE FROM СтатьиДвиженияДенежныхСредств WHERE КодСтатьиДвиженияДенежныхСредств=@КодСтатьиДвиженияДенежныхСредств";

        /// <summary>
        ///     Добавить статью движения денежных средств и получить его идентификатор
        /// </summary>
        public const string INSERT_СтатьиДвиженияДенежныхСредств = @"
INSERT СтатьиДвиженияДенежныхСредств (СтатьяДвиженияДенежныхСредств, КодВидаДвиженияДенежныхСредств, Parent) VALUES (@СтатьяДвиженияДенежныхСредств, @КодВидаДвиженияДенежныхСредств, @Parent)
SELECT SCOPE_IDENTITY()";

        /// <summary>
        ///     Обновление наименования расположения
        /// </summary>
        public const string UPDATE_СтатьиДвиженияДенежныхСредств = @"
UPDATE СтатьиДвиженияДенежныхСредств SET СтатьяДвиженияДенежныхСредств=@СтатьяДвиженияДенежныхСредств, КодВидаДвиженияДенежныхСредств=@КодВидаДвиженияДенежныхСредств WHERE КодСтатьиДвиженияДенежныхСредств=@КодСтатьиДвиженияДенежныхСредств
";

        #endregion

        //++++++++++++++++++ Документы ++++++++++++++++++

        #region Типы документов

        /// <summary>
        ///     Строка запроса: Получение типов документов
        /// </summary>
        public const string SELECT_ТипыДокументов = @"
SELECT {0}  
        T0.[КодТипаДокумента], T0.[ТипДокумента], T0.[TypeDoc], T0.[ТОписание], T0.[TDescription],
        T0.[ИмяПредставления], T0.[NameExist], T0.[НаличиеФормы], T0.[URL], T0.[SearchURL], T0.[HelpURL],
        T0.[ТипАвтогенерацииНомера], T0.[АлгоритмАвтогенерацииНомера], T0.[АвтоДата], T0.[УсловиеПохожести],
        T0.[Исходящий], T0.[Финансовый], T0.[Бухгалтерский], T0.[БухгалтерскийСправочник], T0.[СоздаватьЗащищеным], T0.[ТипОтвета],
        T0.[Changed], T0.[Parent], T0.[L], T0.[R], T0.[Изменил], T0.[Изменено]
FROM ТипыДокументов T0 (nolock) ";

        /// <summary>
        ///     Строка запроса: Получение типа документа по ID
        /// </summary>
        public static readonly string SELECT_ID_ТипДокумента = string.Format("{0} WHERE T0.[КодТипаДокумента] = @id",
            string.Format(SELECT_ТипыДокументов, ""));

        /// <summary>
        ///     Сторока запроса: Непосредственные потомки
        /// </summary>
        public static string SELECT_ТипыДокументов_НепосредственныеПотомки =
            @"SELECT * FROM ТипыДокументов WHERE Parent = {0}";

        /// <summary>
        ///     Сторока запроса: Все родители
        /// </summary>
        public static string SELECT_ТипыДокументов_ВсеРодители =
            @"SELECT * FROM ТипыДокументов WHERE L < {0} AND R > {1}";

        /// <summary>
        ///     Строка запроса: Сохранение нового типа
        /// </summary>
        public const string INSERT_ТипДокумента = @"
INSERT INTO ТипыДокументов
    (ТипДокумента, TypeDoc, ТОписание, TDescription, ИмяПредставления, NameExist, НаличиеФормы, URL, SearchURL, HelpURL, ТипАвтогенерацииНомера, 
    АлгоритмАвтогенерацииНомера, АвтоДата, УсловиеПохожести, Исходящий, Финансовый, Бухгалтерский, БухгалтерскийСправочник, СоздаватьЗащищеным, 
    ТипОтвета, Changed)
VALUES
    (@ТипДокумента, @TypeDoc, @ТОписание, @TDescription, @ИмяПредставления, @NameExist, @НаличиеФормы, @URL, @SearchURL, @HelpURL, @ТипАвтогенерацииНомера,
     @АлгоритмАвтогенерацииНомера, @АвтоДата, @УсловиеПохожести, @Исходящий, @Финансовый, @Бухгалтерский, @БухгалтерскийСправочник, @СоздаватьЗащищеным,
    @ТипОтвета,@Changed)";

        /// <summary>
        ///     Строка запроса: Сохранение изменений в существующем типе документов
        /// </summary>
        public const string UPDATE_ТипДокумента = @"
UPDATE  ТипыДокументов
SET     ТипДокумента = @ТипДокумента, TypeDoc = @TypeDoc, ТОписание = @ТОписание, TDescription = @TDescription, ИмяПредставления = @ИмяПредставления, NameExist = @NameExist, НаличиеФормы = @НаличиеФормы, URL = @URL, SearchURL = @SearchURL, HelpURL = @HelpURL, 
        ТипАвтогенерацииНомера = @ТипАвтогенерацииНомера, АлгоритмАвтогенерацииНомера = @АлгоритмАвтогенерацииНомера, АвтоДата = @АвтоДата, УсловиеПохожести = @УсловиеПохожести, Исходящий = @Исходящий, Финансовый = @Финансовый, Бухгалтерский = @Бухгалтерский, 
        БухгалтерскийСправочник = @БухгалтерскийСправочник, СоздаватьЗащищеным = @СоздаватьЗащищеным, ТипОтвета = @ТипОтвета, Changed = @Changed
WHERE   КодТипаДокумента = @КодТипаДокумента";

        /// <summary>
        ///     Строка запроса: Сохранение нового поля типа документа
        /// </summary>
        public const string INSERT_ПолеДокумента = @"
INSERT INTO ПоляДокументов
            (КодТипаДокумента, ПорядокПоляДокумента, ПолеДокумента, ПолеДокументаEN, ПолеДокументаET, КолонкаТаблицы, Обязательность, 
            Рассчетное, КодТипаПоля, ЧислоДесятичныхЗнаков, URLПоиска, МножественныйВыбор, СтрогийПоиск, ПараметрыПоиска, РежимПоискаТипов, 
            ЗаголовокФормыПоиска, СтрокаПодключения, SQLЗапрос, Описание)
VALUES     (@КодТипаДокумента,@ПорядокПоляДокумента,@ПолеДокумента,@ПолеДокументаEN,@ПолеДокументаET,@КолонкаТаблицы,@Обязательность,@Рассчетное,
            @КодТипаПоля,@ЧислоДесятичныхЗнаков,@URLПоиска,@МножественныйВыбор,@СтрогийПоиск,@ПараметрыПоиска,@РежимПоискаТипов,@ЗаголовокФормыПоиска,
            @СтрокаПодключения,@SQLЗапрос,@Описание)";

        /// <summary>
        ///     Строка запроса: Сохранение изменений в существующем поле типа документа
        /// </summary>
        public const string UPDATE_ПолеДокумента = @"
UPDATE  ПоляДокументов
SET     КодТипаДокумента = @КодТипаДокумента, ПорядокПоляДокумента = @ПорядокПоляДокумента, ПолеДокумента = @ПолеДокумента, ПолеДокументаEN = @ПолеДокументаEN, ПолеДокументаET = @ПолеДокументаET, КолонкаТаблицы = @КолонкаТаблицы, Обязательность = @Обязательность, 
        Рассчетное = @Рассчетное, КодТипаПоля = @КодТипаПоля, ЧислоДесятичныхЗнаков = @ЧислоДесятичныхЗнаков, URLПоиска = @URLПоиска, МножественныйВыбор = @МножественныйВыбор, СтрогийПоиск = @СтрогийПоиск, ПараметрыПоиска = @ПараметрыПоиска, РежимПоискаТипов = @РежимПоискаТипов, 
        ЗаголовокФормыПоиска = @ЗаголовокФормыПоиска, СтрокаПодключения = @СтрокаПодключения, SQLЗапрос = @SQLЗапрос, Описание = @Описание
WHERE КодПоляДокумента = @КодПоляДокумента";

        /// <summary>
        ///     Строка запроса: Получение полей документов
        /// </summary>
        public const string SELECT_ПоляДокументов = @"
SELECT  КодПоляДокумента, КодТипаДокумента, ПорядокПоляДокумента, ПолеДокумента, ПолеДокументаEN, ПолеДокументаET, 
        КолонкаТаблицы, Обязательность, Рассчетное, КодТипаПоля, ЧислоДесятичныхЗнаков, URLПоиска, МножественныйВыбор, СтрогийПоиск,
        ПараметрыПоиска, РежимПоискаТипов, ЗаголовокФормыПоиска, СтрокаПодключения, SQLЗапрос, Описание, Изменил, Изменено 
FROM    ПоляДокументов (nolock) ";

        /// <summary>
        ///     Строка запроса: Получение поля документа по ID
        /// </summary>
        public static readonly string SELECT_ID_ПолеДокумента =
            string.Format(@"{0} WHERE КодПоляДокумента = @id", SELECT_ПоляДокументов);

        /// <summary>
        ///     Строка запроса: Получение полей документа по типу документа
        /// </summary>
        public static readonly string SELECT_ПоляДокументов_ТипДокумента =
            string.Format(@"{0} WHERE КодТипаДокумента = @id", SELECT_ПоляДокументов);


        /// <summary>
        ///     Строка запроса: Получение связи типов основание по вытекающим
        /// </summary>
        public const string SELECT_СвязиТиповДокументов_ТипВытекающего =
            @"SELECT * FROM СвязиТиповДокументов (nolock) WHERE КодТипаДокументаВытекающего=@id";

        /// <summary>
        ///     Строка запроса: Получение всех вытекающих типов документа
        /// </summary>
        public static string SELECT_СвязиТиповДокументов_Вытекающие = @"
DECLARE @Types TABLE(ТипДокументаВСвязях int, КодТипаДокумента int, SearchType int)
DECLARE @BaseTypeId int

SET @BaseTypeId = @Id

INSERT INTO @Types
-- тип основания - равен
SELECT @BaseTypeId, @BaseTypeId, 0
UNION ALL
-- тип основания - потомок указанного в СвязиТиповДокументов родителя
SELECT Pr.КодТипаДокумента, Ch.КодТипаДокумента, 1
FROM ТипыДокументов Pr (nolock)
	INNER JOIN (SELECT * FROM ТипыДокументов (nolock) WHERE КодТипаДокумента = @BaseTypeId) Ch ON Pr.L <= Ch.L AND Ch.R <= Pr.R
UNION ALL
-- тип основания - синоним потомков
SELECT Syn.КодТипаДокумента, Ch.КодТипаДокумента, 3
FROM ТипыДокументов Pr (nolock)
	INNER JOIN (SELECT * FROM ТипыДокументов (nolock) WHERE КодТипаДокумента = @BaseTypeId) Ch ON Pr.L <= Ch.L AND Ch.R <= Pr.R
	INNER JOIN ТипыДокументов Syn (nolock) ON Syn.Parent = Pr.Parent

SELECT DISTINCT
	@BaseTypeId as КодТипаДокументаОснования,
	Связи.ТипДокументаОснования,	-- м.б. дочерним, синонимом - зависит от РежимПоискаОснования
	Связи.КодТипаДокументаВытекающего,
	Связи.ТипДокументаВытекающего,
	Связи.ВыводитьСписокВОсновании,
	Связи.ПорядокВыводаВОсновании,
	Связи.ТипСвязи,
	Связи.РежимПоискаОснования,
	Связи.КодПоляДокумента,
	ПоляДокументов.ПолеДокумента, ПоляДокументов.ПолеДокументаEn
FROM СвязиТиповДокументов Связи (nolock)
	INNER JOIN @Types Типы ON Типы.ТипДокументаВСвязях = КодТипаДокументаОснования AND Типы.SearchType = РежимПоискаОснования AND Типы.КодТипаДокумента = @BaseTypeId
	INNER JOIN ПоляДокументов (nolock) ON ПоляДокументов.КодПоляДокумента = Связи.КодПоляДокумента
	INNER JOIN ТипыДокументов (nolock) ON ТипыДокументов.КодТипаДокумента = Связи.КодТипаДокументаВытекающего
WHERE ВыводитьСписокВОсновании = 1 AND ТипыДокументов.НаличиеФормы >= 1";

        #endregion

        #region Документы

        /// <summary>
        ///     Лица Документа По Бизнес Проекту
        /// </summary>
        public const string SELECT_ID_ЛицаДокументаПоБизнесПроекту = @"
SELECT КодЛица 
FROM   Справочники.dbo.vwЛица Лица (nolock)
WHERE  КодБизнесПроекта IS NOT NULL AND EXISTS(SELECT * FROM vwЛицаДокументов ЛицаДокументов (nolock) WHERE ЛицаДокументов.КодДокумента = @id AND ЛицаДокументов.КодЛица = Лица.КодЛица)";


        /// <summary>
        ///     Хранимая процедура: Создание номера документа
        /// </summary>
        public const string SP_СозданиеНомераДокумента = "sp_СозданиеНомераДокумента";

        /// <summary>
        ///     Строка запроса: Получение документов
        /// </summary>
        public const string SELECT_Документы = @"
SELECT {0}  T0.[КодДокумента], T0.[КодТипаДокумента], T0.[НазваниеДокумента], T0.[НомерДокумента], T0.[ДатаДокумента], T0.[Описание],
            T0.[КодИзображенияДокументаОсновного], T0.[НомерInt], T0.[НомерДокументаRL], T0.[НомерДокументаRLReverse], T0.[Защищен], T0.[Изменил], T0.[Изменено]
            ,T1.[ТипДокумента], T1.[TypeDoc]
FROM vwДокументы T0 (nolock) 
LEFT JOIN ТипыДокументов T1 ON T1.[КодТипаДокумента] = T0.[КодТипаДокумента]
";

        /// <summary>
        ///     Строка запроса: Последнее изменение документа
        /// </summary>
        public const string SELECT_Документ_LastChanged =
            @"SELECT T0.Изменено FROM vwДокументы T0 (nolock) WHERE T0.КодДокумента = @id";

        /// <summary>
        ///     Строка запроса: Поиск документа по коду
        /// </summary>
        public static readonly string SELECT_ID_Документ =
            string.Format(@"{0} WHERE T0.КодДокумента = @id", string.Format(SELECT_Документы, ""));

        /// <summary>
        ///     Строка запроса: Поиск всех документов основания
        /// </summary>
        public static readonly string SELECT_ВсеОснования =
            string.Format(
                @"{0} INNER JOIN vwСвязиДокументов (nolock) ON T0.КодДокумента= vwСвязиДокументов.КодДокументаОснования WHERE vwСвязиДокументов.КодДокументаВытекающего=",
                string.Format(SELECT_Документы, ""));

        /// <summary>
        ///     Строка запроса: Получить все типы договоров
        /// </summary>
        public const string SELECT_ТипыДоговоров = @"
SELECT  Child.* 
FROM    ТипыДокументов Parent INNER JOIN
        ТипыДокументов Child ON Parent.L <= Child.L AND Parent.R >= Child.R
WHERE   Parent.КодТипаДокумента = 2039 AND Child.ТипДокумента IS NOT NULL
";

        /// <summary>
        ///     Строка запроса: Получить все типы приложений
        /// </summary>
        public const string SELECT_ТипыПриложений = @"
SELECT  Child.* 
FROM    ТипыДокументов Parent INNER JOIN
        ТипыДокументов Child ON Parent.L <= Child.L AND Parent.R >= Child.R
WHERE   Parent.КодТипаДокумента = 2110 AND Child.ТипДокумента IS NOT NULL
";

        /// <summary>
        ///     Строка запроса: Получить все вытекающие документы
        /// </summary>
        public static string SELECT_ВсеВытекающие(int id, int fieldId)
        {
            var getField = string.Empty;
            if (fieldId > 0)
                getField = " AND vwСвязиДокументов.КодПоляДокумента=" + fieldId;

            return string.Format(@"{0} INNER JOIN 
vwСвязиДокументов (nolock) ON T0.КодДокумента = vwСвязиДокументов.КодДокументаВытекающего 
WHERE vwСвязиДокументов.КодДокументаОснования = {1} {2} 
ORDER BY ПорядокВытекающего", string.Format(SELECT_Документы, ""), id, getField);
        }

        /// <summary>
        /// Строка запроса: Обновление описания документа
        /// </summary>
        public const string UPDATE_ID_Документы_Описание = @"UPDATE vwДокументы SET Описание = @Описание WHERE КодДокумента = @id";

        /// <summary>
        ///     Хранимая процедура: Создание ДокументовДанных
        /// </summary>
        public const string SP_ДокументыДанные_InsUpd = "sp_ДокументыДанные_InsUpd";

        /// <summary>
        ///     Хранимая процедура: Поиск похожих документов
        /// </summary>
        public const string SP_ПохожиеДокументы = "sp_ПохожиеДокументы";

        /// <summary>
        ///     Строка запроса: Удаление ДокументовДанных(далее триггер вызовет deleter для документа, если была только электронная
        ///     форма)
        /// </summary>
        public const string DELETE_ID_ДокументДанные = "DELETE FROM dbo.vwДокументыДанные WHERE КодДокумента = @id";

        /// <summary>
        ///     Строка запроса: Экспортирован ли документ в 1С
        /// </summary>
        public const string SELECT_ID_ДокументЭкспортированВ1С = @"
IF EXISTS(SELECT 1 FROM БухПараметрыДокумента (nolock) WHERE КодДокумента = @id) 
    SELECT 1 
ELSE 
BEGIN 
    IF EXISTS(SELECT 1 FROM БухПараметрыДоговоров (nolock) WHERE КодДокумента = @id) 
        SELECT 1 
    ELSE
        SELECT 0 
END";

        /// <summary>
        ///     Строка запроса: Получение строки из таблицы "ДокументыДанные" по ID
        /// </summary>
        public const string SELECT_ID_ДокументДанные = @"
SELECT  КодДокумента, Изменил, Изменено,
        КодЛица1, КодЛица2, КодЛица3, КодЛица4, КодЛица5, КодЛица6, 
        КодСклада1, КодСклада2, КодСклада3, КодСклада4, 
        КодРесурса1, КодРесурса2, 
        КодСотрудника1, КодСотрудника2, КодСотрудника3,
        КодРасположения1, КодБазисаПоставки, КодВидаТранспорта, КодМестаХранения, КодЕдиницыИзмерения, КодСтавкиНДС, КодТУзла1, КодТУзла2, КодТерритории, КодСтатьиБюджета,
        Дата2, Дата3, Дата4, Дата5, 
        Flag1, Flag2, 
        Int1, Int2, Int3, Int4, Int5, Int6, Int7,
        Text50_1, Text50_2, Text50_3, Text50_4, Text50_5, Text50_6, Text50_7, Text50_8, Text50_9, Text50_10, Text50_11, Text50_12, Text50_13,
        Text100_1, Text100_2, Text100_3, Text100_4, Text100_5, Text100_6,
        Text300_1, Text300_2, Text300_3, Text300_4, Text300_5, Text300_6, Text300_7, Text300_8, Text300_9, 
        Text1000_1, Text1000_2,
        Money1, Money2, Money3, Money4, Money5, Money6, Money7, Money8, Money9,
        Decimal1, Decimal2, Float1,
        ТекстДокумента
FROM vwДокументыДанные T0 (nolock) WHERE T0.КодДокумента = @id";

        /// <summary>
        ///     Строка запроса: Получение строки из таблицы "СвязиДокументов"
        /// </summary>
        public const string SELECT_СвязиДокументов = @"
SELECT  КодСвязиДокументов, КодДокументаОснования, КодДокументаВытекающего, КодПоляДокумента, ПорядокОснования, ПорядокВытекающего, Изменил, Изменено
FROM    vwСвязиДокументов (nolock) {0}";

        /// <summary>
        ///     Строка запроса: Получение строки из таблицы "СвязиДокументов" по ID
        /// </summary>
        public static readonly string SELECT_ID_СвязиДокументов =
            string.Format(SELECT_СвязиДокументов, "WHERE КодСвязиДокументов=@id");

        /// <summary>
        ///     Строка запроса: Получение связей документов по коду документа вытекающего
        /// </summary>
        public static readonly string SELECT_СвязиДокументов_ПоВытекающему =
            string.Format(SELECT_СвязиДокументов, "WHERE КодДокументаВытекающего=@id");

        /// <summary>
        ///     Строка запроса: Получение связей документов по коду документа основания
        /// </summary>
        public static readonly string SELECT_СвязиДокументов_ПоОснованию =
            string.Format(SELECT_СвязиДокументов, "WHERE КодДокументаОснования=@id");

        /// <summary>
        ///     Строка запроса: Получение связей документов по коду документа вытекающего и коду поля документа
        /// </summary>
        public static readonly string SELECT_СвязиДокументов_ПоВытекающему_ПоПолю =
            string.Format(SELECT_СвязиДокументов,
                "WHERE КодДокументаВытекающего= @IdDoc AND КодПоляДокумента = @IDField  ORDER BY ПорядокОснования");


        /// <summary>
        ///     Строка запроса: Получить ID строки из таблицы СвязиДокументов
        /// </summary>
        public static readonly string SELECT_СвязиДокументов_ПоОснованию_ПоВытекающему =
            string.Format(SELECT_СвязиДокументов,
                "WHERE КодДокументаОснования = @ParentDocID AND КодДокументаВытекающего = @ChildDocID");

        /// <summary>
        ///     Строка запроса: Получить ID строки из таблицы СвязиДокументов и поля таблицы
        /// </summary>
        public static readonly string SELECT_СвязиДокументов_ПоОснованию_ПоВытекающему_ПоПолю =
            string.Format(SELECT_СвязиДокументов,
                "WHERE КодДокументаОснования = @ParentDocID AND КодДокументаВытекающего = @ChildDocID AND КодПоляДокумента = @FildID");

        /// <summary>
        ///     Строка запроса: Проверка вытекающего документа
        /// </summary>
        public const string SELECT_TEST_СвязиДокументовВытекающие = @"
IF EXISTS(SELECT NULL FROM Инвентаризация.dbo.fn_SplitInts(@Коды) AS T1 LEFT JOIN (SELECT * FROM Инвентаризация.dbo.fn_SplitInts(@КодыОснований) AS T1 CROSS APPLY
fn_ВсеВытекающие( T1.value)) AS T2 ON T1.value=T2.КодДокумента WHERE T2.КодДокумента IS NULL)
SELECT 0
ELSE
SELECT 1";

        /// <summary>
        ///     Строка запроса: Получение всех вытекающих документов из всех оснований
        /// </summary>
        public const string SELECT_СвязиДокументовВытекающиеИзОснований = @"
SELECT DISTINCT КодДокумента FROM Инвентаризация.dbo.fn_SplitInts(@Коды) AS T1 CROSS APPLY fn_ВсеВытекающие(T1.value)";

        /// <summary>
        ///     Строка запроса: Получение всех оснований, для которых уже нет других оснований, из всех вытекающих документов
        /// </summary>
        public const string SELECT_СвязиДокументовОснованияДляВытекающих = @"
SELECT DISTINCT КодДокумента FROM Инвентаризация.dbo.fn_SplitInts(@Коды) AS T1 CROSS APPLY fn_ВсеОснования(T1.value) 
WHERE NOT EXISTS(SELECT NULL FROM fn_ВсеОснования(КодДокумента))";


        /// <summary>
        ///     Строка запроса: Получение всех вытекающих документов из всех оснований
        /// </summary>
        public static string SELECT_СвязиДокументовВытекающиеИзОснованийПоТипу(string id, string fieldId,
            string typeList)
        {
            return
                string.Format(
                    @"{0} INNER JOIN vwСвязиДокументов (nolock) ON T0.КодДокумента = vwСвязиДокументов.КодДокументаВытекающего 
                    WHERE vwСвязиДокументов.КодДокументаОснования = {1} 
                    AND vwСвязиДокументов.КодПоляДокумента = {2} 
                    AND T0.КодТипаДокумента IN ({3})
                    ORDER BY ПорядокВытекающего",
                    string.Format(SELECT_Документы, ""), id, fieldId, typeList);
        }

        /// <summary>
        ///     Строка запроса: Получение всех оснований, для которых уже нет других оснований, из всех вытекающих документов
        /// </summary>
        public static string SELECT_СвязиДокументовОснованияДляВытекающихПоТипу(string id, string fieldId,
            string typeList)
        {
            return
                string.Format(
                    @"{0} INNER JOIN vwСвязиДокументов (nolock) ON КодДокумента = vwСвязиДокументов.КодДокументаОснования 
                    WHERE vwСвязиДокументов.КодДокументаВытекающего = {1} 
                    AND vwСвязиДокументов.КодПоляДокумента = {2} 
                    AND T0.КодТипаДокумента IN ({3})
                    ORDER BY ПорядокВытекающего",
                    string.Format(SELECT_Документы, ""), id, fieldId, typeList);
        }

        /// <summary>
        ///     Хранимая процедура: Создание связи с документами
        /// </summary>
        public const string SP_MakeDocsLink = "sp_MakeDocsLink";

        /// <summary>
        ///     Строка запроса: Удаление связи документов
        /// </summary>
        public const string DELETE_ID_СвязиДокументов =
            "DELETE FROM vwСвязиДокументовПорядок WHERE КодСвязиДокументов = @id";

        /// <summary>
        ///     Строка запроса: Удаление связи документов
        /// </summary>
        public const string DELETE_СвязиДокументов_ПоОснованию_ПоВытекающему_ПоПолю = @"
DELETE FROM vwСвязиДокументов 
WHERE КодДокументаОснования = @ParentDocID AND КодДокументаВытекающего = @ChildDocID AND КодПоляДокумента = @FildID";

        /// <summary>
        ///     Строка запроса: Получение строк из таблицы ПодписиДокументов
        /// </summary>
        public const string SELECT_ПодписиДокументов = @"
SELECT  КодПодписиДокумента, КодДокумента, КодИзображенияДокумента, КодСотрудника, КодСотрудникаЗА, Дата, ТипПодписи,
        КодШтампа, Page, X, Y, Zoom, Rotate
FROM    ПодписиДокументов
";

        /// <summary>
        ///     Строка запроса: Удаление подписи документа
        /// </summary>
        public const string DELETE_ID_ПодписьДокумента =
            @"DELETE ПодписиДокументов WHERE КодПодписиДокумента = @SignId";

        /// <summary>
        ///     Строка запроса: Получение строки из таблицы "ПодписиДокументов" по ID
        /// </summary>
        public static readonly string SELECT_ID_ПодписьДокумента =
            string.Concat(SELECT_ПодписиДокументов, "WHERE КодПодписиДокумента = @id");

        /// <summary>
        ///     Строка запроса: Получить сообщения подписи
        /// </summary>
        public const string SELECT_СообщенияПодписи = @"
DECLARE @КодСотрудника int, @Язык char(2) 
SET @КодСотрудника = 0 
SET @Язык='en' 

SELECT @КодСотрудника = КодСотрудника, @Язык = Язык FROM Инвентаризация.dbo.Сотрудники (nolock) WHERE SID = SUSER_SID() 

SELECT  TOP 1 CASE WHEN @Язык = 'ru' THEN ТекстСообщения ELSE CASE WHEN @Язык = 'et' THEN ТекстСообщенияEt ELSE ТекстСообщенияEn END END ТекстСообщения 
FROM    ТипыДокументовСообщенияПодписи (nolock) 
WHERE   КодСотрудника IN (0, @КодСотрудника) AND КодТипаДокумента = @DocTypeId AND ТипПодписи = @SignType 
ORDER BY КодСотрудника DESC";

        /// <summary>
        ///     Строка запроса: Добавить подпись документа
        /// </summary>
        public const string INSERT_ПодписьДокумента = @"
INSERT  ПодписиДокументов(КодДокумента,КодСотрудника,КодСотрудникаЗА,Дата,ТипПодписи)
VALUES (@КодДокумента, @КодСотрудника, @КодСотрудникаЗА, GETUTCDATE(), @ТипПодписи)";

        /// <summary>
        ///     Строка запроса: Загрузка настроек docview
        /// </summary>
        public const string SELECT_НастройкиDocView = @"
IF EXISTS(SELECT 1 FROM vwНастройки)
    SELECT * FROM vwНастройки 
ELSE
    EXEC sp_Настройки";

        /// <summary>
        ///     Строка запроса: Сохранение(Update) настроек docview
        /// </summary>
        public const string UPDATE_НастройкиDocView = @"
UPDATE  vwНастройки 
SET     КодЛица									= @КодЛица,
        ПорядокГруппировки						= @ПорядокГруппировки,
        КодыДокументовСвязующих					= @КодыДокументовСвязующих,
        УведомлениеСообщения					= @УведомлениеСообщения,
        ВремяОтметкиПрочтения					= @ВремяОтметкиПрочтения,
        ДокументыПодуровней						= @ДокументыПодуровней,
        ФильтрДатыАрхивирования					= @ФильтрДатыАрхивирования,
        ФильтрДатыДокумента						= @ФильтрДатыДокумента,
        ФильтрДатыСоздания						= @ФильтрДатыСоздания,
        ПодтвУдаления							= @ПодтвУдаления,
        ПодтвГрупповыхОпераций					= @ПодтвГрупповыхОпераций,
        ПоказыватьНовости						= @ПоказыватьНовости,
        СохранениеДобавитьВРаботу				= @СохранениеДобавитьВРаботу,
        СохранениеОткрытьСохранённый			= @СохранениеОткрытьСохранённый,
        СохранениеПослатьСообщение				= @СохранениеПослатьСообщение,
        ПодписьВыполненоСообщение				= @ПодписьВыполненоСообщение,
        ФаксыВходящиеТолькоНеСохранённые		= @ФаксыВходящиеТолькоНеСохранённые,
        ФаксыОтправленныеТолькоНеСохранённые	= @ФаксыОтправленныеТолькоНеСохранённые,
        ПереходНаСледующийПриОтправкеСообщения	= @ПереходНаСледующийПриОтправкеСообщения,
        ИскатьНесколькоДокументовПоШтрихкоду	= @ИскатьНесколькоДокументовПоШтрихкоду,
        ЛичныеСпискиРассылкиПоказыватьПервыми   = @ЛичныеСпискиРассылкиПоказыватьПервыми,
        ПрочитыватьСообщениеПриЗавершенииРаботы = @ПрочитыватьСообщениеПриЗавершенииРаботы";


        /// <summary>
        ///     Строка запроса: Получение таблицы лица документов
        /// </summary>
        public const string SELECT_ЛицаДокументов = @"
SELECT  КодЛицаДокумента, КодДокумента, КодЛица, Положение, Изменил, Изменено
FROM    vwЛицаДокументов T0 (nolock)";


        /// <summary>
        ///     Строка запроса: Получение Коды лиц подкументов по ID и условием Положение>0
        /// </summary>
        public static readonly string SELECT_ЛицаДокументов_ПоДокументу_ПоПоложению =
            string.Concat(SELECT_ЛицаДокументов, "WHERE Положение > 0 AND КодДокумента = @Id");

        /// <summary>
        ///     Строка запроса: Получение Коды лиц подкументов по ID
        /// </summary>
        public static readonly string SELECT_ЛицаДокументов_ПоДокументу =
            string.Concat(SELECT_ЛицаДокументов, "WHERE КодДокумента = @Id");

        /// <summary>
        ///     Строка запроса: Получение таблицы лица документов по ID
        /// </summary>
        public static string SELECT_ID_ЛицаДокументов =
            string.Concat(SELECT_ЛицаДокументов, "WHERE КодЛицаДокумента = @Id");

        /// <summary>
        ///     Строка запроса: Получение ПоследнийДокументПоТипу
        /// </summary>
        public static string SELECT_ПоследнийДокументПоТипу = @"
SELECT TOP 1 КодДокумента, КодЛица3, КодЛица4
FROM vwДокументыДокументыДанные
INNER JOIN vwСвязиДокументов ON vwСвязиДокументов.КодДокументаВытекающего = vwДокументыДокументыДанные.КодДокумента
WHERE КодТипаДокумента = @КодТипаДокумента
AND КодПоляДокумента = 707
AND КодЛица1 = @КодЛица1
AND КодЛица2 = @КодЛица2
AND КодДокументаОснования = @КодДокументаОснования";

        /// <summary>
        ///     Строка запроса: Получение ПоследнийДокументПоТипу
        /// </summary>
        public static string SELECT_ПоследнийИзмДокументПоТипу = @"
SELECT TOP 1 vwДокументы.КодДокумента, Text50_2,Text50_11,Text50_3,Text50_12,Text50_13,Text100_3
FROM vwДокументы
INNER JOIN vwДокументыДокументыДанные ON vwДокументы.КодДокумента = vwДокументыДокументыДанные.КодДокумента
WHERE vwДокументы.КодТипаДокумента = @КодТипаДокумента
AND КодЛица1 = @КодЛица1
AND vwДокументы.КодДокумента <> @КодДокумента
ORDER BY vwДокументы.Изменено DESC";

        /// <summary>
        ///     Удаление записи ОтправкаВагоновВыгрузка по GUID
        /// </summary>
        public static string DELETE_ОтправкаВагоновВыгрузка = @"DELETE FROM ОтправкаВагоновВыгрузка WHERE guid=@guid";

        #endregion

        #region Наборы

        /// <summary>
        ///     Строка запроса: Список ОтправкаВагонов
        /// </summary>
        public const string SELECT_КодыДвиженияНаСкладе =
            @"SELECT DISTINCT КодДвиженияНаСкладе FROM vwДвиженияНаСкладах d (nolock)
								WHERE d.КодДвиженияНаСкладе IN({0}) AND
								EXISTS(SELECT * FROM vwОтправкаВагоновУчастки t (nolock) WHERE d.КодОтправкиВагона=t.КодОтправкиВагона AND t.НомерДокумента like '%'+@НомерДокумента+'%')";

        /// <summary>
        ///     Строка запроса: Список ОтправкаВагонов
        /// </summary>
        public const string SELECT_DvDoc = @"
SELECT d.КодДвиженияНаСкладе,d.КодОтправкиВагона,ISNULL(t.ОтправкаВагона,'') ОтправкаВагона, d.Количество* Справочники.dbo.fn_unitConverter(@КодРесурса,d.КодЕдиницыИзмерения,@КодЕдиницыИзмерения) Количество
FROM vwДвиженияНаСкладах d (nolock)
	LEFT JOIN vwОтправкаВагонов t ON d.КодОтправкиВагона = t.КодОтправкиВагона
WHERE d.КодРесурса=@КодРесурса AND d.КодДокумента=@КодДокумента ORDER BY ОтправкаВагона
";

        /// <summary>
        ///     Строка запроса: ОтправкаВагонов по ресурсу и документу (плательщик)
        /// </summary>
        public const string SELECT_NaborDocPayer = @"
SELECT n.КодНабора,КодДвиженияНаСклад КодДвиженияВДокументе,КодДвиженияСоСклада КодДвиженияВНаборе,ISNULL(t.ОтправкаВагона,'') ОтправкаВагона,n.Количество*Справочники.dbo.fn_unitConverter(@КодРесурса,null,@КодЕдиницыИзмерения) Количество
FROM Наборы n
	INNER JOIN  vwДвиженияНаСкладах d (nolock) ON n.КодДвиженияНаСклад = d.КодДвиженияНаСкладе
	LEFT JOIN vwОтправкаВагонов t (nolock) ON t.КодОтправкиВагона = d.КодОтправкиВагона
WHERE d.КодРесурса=@КодРесурса AND d.КодДокумента=@КодДокумента
";

        /// <summary>
        ///     Строка запроса: ОтправкаВагонов по ресурсу и документу (получатель)
        /// </summary>
        public const string SELECT_NaborDocShipper = @"
SELECT n.КодНабора,КодДвиженияСоСклада КодДвиженияВДокументе,КодДвиженияНаСклад КодДвиженияВНаборе,ISNULL(t.ОтправкаВагона,'') ОтправкаВагона,n.Количество*Справочники.dbo.fn_unitConverter(@КодРесурса,null,@КодЕдиницыИзмерения) Количество
FROM Наборы n
	INNER JOIN  vwДвиженияНаСкладах d (nolock) ON n.КодДвиженияСоСклада = d.КодДвиженияНаСкладе
	LEFT JOIN vwОтправкаВагонов t (nolock) ON t.КодОтправкиВагона = d.КодОтправкиВагона
WHERE d.КодРесурса=@КодРесурса AND d.КодДокумента=@КодДокумента
";

        #endregion

        #region Позиции документов

        /// <summary>
        ///     Строка запроса: Получение связи документов и позиции по ID
        /// </summary>
        public const string SELECT_ID_ПозицияЗаявкиМТРСвязи =
            "SELECT * FROM vwПозицииЗаявокМТРСвязи (nolock) WHERE КодПозицииЗаявокМТРСвязи = @id";

        /// <summary>
        ///     Строка запроса: Сохранить связь ПозицииЗаявокМТР и Документы
        /// </summary>
        public const string INSERT_ПозицииЗаявокМТРСвязи = @"
INSERT INTO vwПозицииЗаявокМТРСвязи (КодПозицииЗаявокМТР, КодДокументаОснованияОплаты, Количество)
VALUES (@КодПозицииЗаявокМТР, @КодДокументаОснованияОплаты, @Количество)

SET @КодПозицииЗаявокМТРСвязи = SCOPE_IDENTITY()";

        /// <summary>
        ///     Строка запроса: Удалить связь ПозицииЗаявокМТР и Документы
        /// </summary>
        public const string DELETE_ID_ПозицияЗаявкиМТРСвязи =
            "DELETE FROM vwПозицииЗаявокМТРСвязи WHERE КодПозицииЗаявокМТРСвязи = @id";

        /// <summary>
        ///     Строка запроса: Получение оказанных услуг по ID
        /// </summary>
        public const string SELECT_ID_ОказаннаяУслуга =
            "SELECT * FROM vwОказанныеУслуги (nolock) WHERE КодОказаннойУслуги = @id";

        /// <summary>
        ///     Строка запроса: Получение движения на складах по ID
        /// </summary>
        public const string SELECT_ID_ДвижениеНаСкладе =
            "SELECT * FROM vwДвиженияНаСкладах (nolock) WHERE КодДвиженияНаСкладе = @id";

        /// <summary>
        ///     Строка запроса: Получение позицию претензии по ID
        /// </summary>
        public const string SELECT_ID_ПозицияПретензии =
            "SELECT * FROM vwПозицииПретензий (nolock) WHERE КодПозицииПретензии = @id";


        /// <summary>
        ///     Строка запроса: Получение позиции договора по ID
        /// </summary>
        public const string SELECT_ID_ПозицияДоговора =
            "(SELECT * FROM vwПозицииДоговоров (nolock) WHERE КодПозицииДоговора = @id) T0";

        /// <summary>
        ///     Строка запроса: Получение позиции договора по ID
        /// </summary>
        public const string SELECT_ID_ПозицииДоговораПоРесурсу =
            "(SELECT p.* FROM vwПозицииДоговоров p INNER JOIN Справочники.dbo.Ресурсы r ON p.КодРесурса=r.КодРесурса WHERE КодПозицииДоговора = @id) T0";

        #endregion

        #region Транзакции

        /// <summary>
        ///     Строка запроса: Получение строк из таблицы "Транзакции"
        /// </summary>
        public const string SELECT_Транзакции = @"
SELECT  КодТранзакции, КодТипаТранзакции, КодГруппыТиповТранзакций, Дата,
        КодДокументаОснования, КодДокументаПодтверждения, КодДокументаДоговора, КодДокументаПриложения, КодДокументаСФ,
        КодЛицаДО, КодЛицаПОСЛЕ, КодСкладаДО, КодСкладаПОСЛЕ, КодХранителяДо, КодХранителяПосле, КодБизнесПроектаДО, КодБизнесПроектаПОСЛЕ,
        СуммаРуб, КодВалюты, Сумма, КодРесурса, Количество, Примечание, Изменил, Изменено
FROM    vwТранзакции (nolock) 
";

        /// <summary>
        ///     Строка запроса: Получение строку из таблицы "Транзакции" по ID
        /// </summary>
        public static string SELECT_ID_Транзакция = string.Concat(SELECT_Транзакции, "WHERE КодТранзакции = @id");

        /// <summary>
        ///     Строка запроса: Получение строку из таблицы "Транзакции" по полю КодДокументаПодтверждения
        /// </summary>
        public static string SELECT_Транзакции_ПоДокументуПодтверждения =
            string.Concat(SELECT_Транзакции, "WHERE КодДокументаПодтверждения = @CodeDoc");

        /// <summary>
        ///     Строка запроса: Удаление транзакций по коду документа подтверждения
        /// </summary>
        public const string DELETE_Транзакции_ПоДокументуПодтверждения =
            "DELETE vwТранзакции WHERE КодДокументаПодтверждения=@CodeDoc";

        /// <summary>
        ///     Строка запроса: Получение строк из таблицы "Типы транзакции"
        /// </summary>
        public const string SELECT_ТипыТранзакций = @"
SELECT  КодТипаТранзакции, ТипТранзакции, ТипТранзакцииEN, КодГруппыТиповТранзакций, ГруппаАктаСверки, ПорядокАктаСверки, ОписаниеАктаСверки
FROM    ТипыТранзакций (nolock) ";

        /// <summary>
        ///     Строка запроса: Получение строк из таблицы "Типы транзакции" по Id
        /// </summary>
        public static string SELECT_ID_ТипТранзакции =
            string.Concat(SELECT_ТипыТранзакций, "WHERE КодТипаТранзакции = @id");

        #endregion

        #region Шаблон

        /// <summary>
        ///     Строка запроса: Шаблон печатной формы по ID шаблона
        /// </summary>
        public const string SELECT_ID_ШаблонПечатнойФормы = @"
SELECT  КодЛица, КодКонтрагента, КодШаблонаПечатнойФормы, НазваниеШаблона, НазваниеШаблонаЛат
FROM    vwШаблоныПечатныхФорм
WHERE   КодШаблонаПечатнойФормы = @Id";

        /// <summary>
        ///     Строка запроса: Шаблоны печатных форм документов
        /// </summary>
        public const string SELECT_ШаблоныПечатныхФорм = @"
SELECT  {0} КодЛица, КодКонтрагента, КодШаблонаПечатнойФормы, НазваниеШаблона, НазваниеШаблонаЛат
FROM    vwШаблоныПечатныхФорм (nolock) ";

        #endregion

        #region ДополнительныеФильтры

        /// <summary>
        ///     Строка запроса: ДополнительныеФильтрыПриложений
        /// </summary>
        public const string SELECT_ДополнительныеФильтрыПриложений = @"
DECLARE @lang char(2)
SELECT @lang=Язык FROM Сотрудники WHERE SID=SUSER_SID()
SELECT {0} КодУсловия, CASE WHEN @lang <> 'ru' THEN УсловиеЛат ELSE Условие END AS Условие
FROM    НастройкиУсловия T0 WHERE T0.ТипУсловия=@ТипУсловия";

        /// <summary>
        ///     Строка запроса: ДополнительныеФильтрыПриложений
        /// </summary>
        public const string SELECT_IDs_ДополнительныеФильтрыПриложений = @"
SELECT {0} [SQL] AS Запрос
FROM    НастройкиУсловия T0 WHERE КодУсловия IN (SELECT value FROM Инвентаризация.dbo.fn_SplitInts(@КодУсловия))";

        #endregion

        #region Заявление на отпуск

        /// <summary>
        ///     Получить список видов отпуска
        /// </summary>
        public const string SELECT_ВидыОтпуска = @"
SELECT КодВидаОтпуска, CASE WHEN 'ru'=(SELECT TOP(1) Язык FROM Сотрудники WHERE SID=SUSER_SID()) THEN ВидОтпуска + ISNULL(' [' + NULLIF(Примечание,'') + ']','') ELSE ВидОтпускаLat END AS ВидОтпуска 
FROM ВидыОтпуска";

        /// <summary>
        ///     Получить вид отпуска
        /// </summary>
        public const string SELECT_ID_ВидОтпуска = @"
SELECT КодВидаОтпуска, CASE WHEN 'ru'=(SELECT TOP(1) Язык FROM Сотрудники WHERE SID=SUSER_SID()) THEN ВидОтпуска + ISNULL(' [' + NULLIF(Примечание,'') + ']','') ELSE ВидОтпускаLat END AS ВидОтпуска 
FROM ВидыОтпуска
WHERE КодВидаОтпуска = @id";

        /// <summary>
        ///     Получить список руководителей организации
        /// </summary>
        public const string SELECT_Руководитель = @"
SELECT КодЛицаПотомка
FROM vwСвязиЛиц
WHERE От<=@ДатаДокумента AND До>@ДатаДокумента AND КодЛицаРодителя=@Организация AND Параметр=1";

        /// <summary>
        ///     Проверка того что одним из мест работы сотрудника является указанное лицо
        /// </summary>
        public const string SELECT_ТестМестоРаботыСотрудника = @"
IF EXISTS (SELECT NULL FROM Сотрудники C
INNER JOIN vwДолжности Д ON C.КодСотрудника=Д.КодСотрудника
WHERE C.КодСотрудника=@КодСотрудника AND Д.КодЛица = @КодЛица)
SELECT 1";

        /// <summary>
        ///     Проверка того что одним из мест работы лица Кто является указанное лицо Где
        /// </summary>
        public const string SELECT_ТестМестоРаботыЛица = @"
IF EXISTS (SELECT NULL FROM Сотрудники C
INNER JOIN vwДолжности Д ON C.КодСотрудника=Д.КодСотрудника
WHERE C.КодЛица=@КодЛицаКто AND Д.КодЛица = @КодЛицаГде)
SELECT 1";

        /// <summary>
        ///     Получить количества праздников в указанном периоде времени
        /// </summary>
        public const string SELECT_Праздники = @"
DECLARE @L int, @R int;
SELECT @L=L, @R=R FROM Территории WHERE КодТерритории=@КодТерритории;
SELECT COUNT(DISTINCT Дата) FROM Праздники INNER JOIN Территории ON Праздники.КодТерритории = Территории.КодТерритории
    WHERE Дата >= CONVERT(date,@От) AND Дата < CONVERT(date,@До) AND L<=@L AND R>=@R AND Праздник=1";

        /// <summary>
        ///     Получить первый рабочий день начиная с указанного дня
        /// </summary>
        public const string SELECT_ПервыйРабочийДень = @"
DECLARE @ПервыйРабочийДень datetime = @ПервыйДень
WHILE @ПервыйРабочийДень<'99991231'--Максимальное значение даты
BEGIN
IF dbo.fn_РабочиеДни(@ПервыйРабочийДень, @ПервыйРабочийДень+1, @КодТерритории)>0 BREAK
SET @ПервыйРабочийДень+=1
END
SELECT @ПервыйРабочийДень";

        /// <summary>
        ///     Получить список замещений сотрудников по указанному документу
        /// </summary>
        public const string SELECT_ЗамещенияПоДокументу = @"
SELECT КодЗамещенияСотрудников, От, До, КодСотрудникаЗамещающего, Сотрудники.Сотрудник, ЗамещенияСотрудников.Примечания
FROM ЗамещенияСотрудников
LEFT JOIN Сотрудники ON Сотрудники.КодСотрудника=ЗамещенияСотрудников.КодСотрудникаЗамещающего
WHERE ЗамещенияСотрудников.КодДокумента = @КодДокумента ORDER BY Сотрудники.Сотрудник,От,До";

        /// <summary>
        ///     Вставить одно новое замещение и получить его идентификатор
        /// </summary>
        public const string INSERT_Замещения = @"
INSERT INTO ЗамещенияСотрудников(От,До,КодСотрудникаЗамещаемого,КодСотрудникаЗамещающего,КодДокумента,Примечания) VALUES (@От,@До,@КодСотрудникаЗамещаемого,@КодСотрудникаЗамещающего,@КодДокумента,@Примечания)
SELECT @@IDENTITY";

        /// <summary>
        ///     Изменить одно замещение
        /// </summary>
        public const string UPDATE_Замещения = @"
UPDATE ЗамещенияСотрудников
SET От=@От, До=@До, КодСотрудникаЗамещающего=@КодСотрудникаЗамещающего, КодДокумента=@КодДокумента, Примечания=@Примечания
WHERE КодЗамещенияСотрудников=@КодЗамещенияСотрудников";

        /// <summary>
        ///     Изменить документ замещения
        /// </summary>
        public const string UPDATE_ДокументЗамещения = @"
UPDATE ЗамещенияСотрудников
SET КодДокумента=@КодДокумента
WHERE КодЗамещенияСотрудников=@КодЗамещенияСотрудников";

        /// <summary>
        ///     Удалить одно замещение
        /// </summary>
        public const string DELETE_Замещения = @"
DELETE FROM ЗамещенияСотрудников WHERE КодЗамещенияСотрудников=@КодЗамещенияСотрудников";

        /// <summary>
        ///     Слияние замещений с таблицей новых значений
        /// </summary>
        public const string MERGE_Замещения = @"
MERGE INTO ЗамещенияСотрудников
USING
(VALUES {0})
AS НовыеЗамещения(КодЗамещенияСотрудников, От, До, КодСотрудникаЗамещаемого, КодСотрудникаЗамещающего, КодДокумента, Примечания)
ON НовыеЗамещения.КодЗамещенияСотрудников = ЗамещенияСотрудников.КодЗамещенияСотрудников
WHEN NOT MATCHED THEN INSERT (От, До, КодСотрудникаЗамещаемого, КодСотрудникаЗамещающего, КодДокумента, Примечания) VALUES(НовыеЗамещения.От, НовыеЗамещения.До, НовыеЗамещения.КодСотрудникаЗамещаемого, НовыеЗамещения.КодСотрудникаЗамещающего, НовыеЗамещения.КодДокумента, НовыеЗамещения.Примечания)
WHEN MATCHED THEN UPDATE SET ЗамещенияСотрудников.От=НовыеЗамещения.Oт, ЗамещенияСотрудников.До=НовыеЗамещения.До, ЗамещенияСотрудников.КодСотрудникаЗамещающего=НовыеЗамещения.КодСотрудникаЗамещающего, ЗамещенияСотрудников.КодДокумента=НовыеЗамещения.КодДокумента, ЗамещенияСотрудников.Примечания=НовыеЗамещения.Примечания
WHEN NOT MATCHED BY SOURCE THEN DELETE;";

        #endregion

        #region Указания ИТ

        /// <summary>
        ///     Процедура выполнения указания
        /// </summary>
        public const string SP_ВыполнениеУказанийIT = "sp_ВыполнениеУказанийIT";

        /// <summary>
        ///     Выданное по указанию оборудование
        /// </summary>
        public const string SP_ВыданноеПоУказаниюОборудование = "sp_ВыданноеПоУказаниюОборудование";


        #region Проверки

        /// <summary>
        ///     Проверка наличия указаний на указанного сотрудника и указанное рабочее место
        /// </summary>
        public const string SELECT_2356_CHECK_ДругиеУказанияНаСотрудника = @"
DECLARE @Язык char(2)
SELECT @Язык = Язык FROM Инвентаризация.dbo.Сотрудники WHERE SID=SUSER_SID()

SELECT X.КодДокумента, 
        CASE    WHEN @Язык='ru' THEN ТипыДокументов.ТипДокумента                
                ELSE ТипыДокументов.TypeDoc
        END   + ' №' + X.НомерДокумента + CASE WHEN @Язык='ru' THEN ' от ' ELSE ' dd ' END + CONVERT(varchar, X.ДатаДокумента, 104) Документ     
FROM    (SELECT _КодДокумента КодДокумента, КодТипаДокумента, НомерДокумента, ДатаДокумента
        FROM vwДокументыДокументыДанные (nolock) 
        WHERE КодТипаДокумента=2356 AND КодСотрудника1=@КодСотрудника AND КодРасположения1=@КодРасположения AND КодДокумента <> @КодДокумента) X INNER JOIN 
        ТипыДокументов ON ТипыДокументов.КодТипаДокумента = X.КодТипаДокумента
ORDER BY X.ДатаДокумента";

        /// <summary>
        ///     Проверка унильности логина сотрудника
        /// </summary>
        public const string SELECT_2356_CHECK_УникальныйЛогинСотрудника = @"
SELECT КодСотрудника FROM Инвентаризация.dbo.Сотрудники WHERE КодСотрудника <> @КодСотрудника AND Login = @Login
";

        /// <summary>
        ///     Проверка, что указанный сотрудник работает месте группы сотрудников
        /// </summary>
        public const string SELECT_2356_CHECK_СотрудникНаМестеГруппы = @"
SELECT	Сотрудники.КодСотрудника, Сотрудник 
FROM	Сотрудники INNER JOIN
        РабочиеМеста ON Сотрудники.КодСотрудника = РабочиеМеста.КодСотрудника
WHERE	КодРасположения = @КодРасположения AND РабочиеМеста.КодСотрудника <> @КодСотрудника 
	    AND EXISTS(SELECT * FROM Сотрудники X WHERE X.КодОбщегоСотрудника = Сотрудники.КодСотрудника)
";

        #endregion

        #region Позиции

        /// <summary>
        ///     Строка запроса: Получение позицию указаний папки по ID
        /// </summary>
        public const string SELECT_ID_ПозицияУказанийИТПапки =
            "SELECT * FROM vwПозицииУказанийИТПапки (nolock) WHERE КодПозицииУказанийИТПапка = @id";

        /// <summary>
        ///     Строка запроса: Получение позиций указаний папки по коду документа ID
        /// </summary>
        public const string SELECT_ID_DOC_ПозицииУказанийИТПапки =
            "SELECT * FROM vwПозицииУказанийИТПапки (nolock) WHERE КодДокумента = @id";

        /// <summary>
        ///     Строка запроса: Получение позицию указаний роли по ID
        /// </summary>
        public const string SELECT_ID_ПозицияУказанийИТРоли =
            "SELECT * FROM vwПозицииУказанийИТРоли (nolock) WHERE КодПозицииУказанийИТРоль = @id";

        /// <summary>
        ///     Строка запроса: Получение позиций указаний роли по коду документа ID
        /// </summary>
        public const string SELECT_ID_DOC_ПозицииУказанийИТРоли =
            "SELECT * FROM vwПозицииУказанийИТРоли (nolock) WHERE КодДокумента = @id";

        /// <summary>
        ///     Подчиненный запрос без SELECT
        ///     Строка запроса: Получение позиций указаний роли по коду документа ID
        /// </summary>
        public const string SUBQUERY_ID_DOC_ПозицииУказанийИТРоли = @"
(SELECT	Позиции.КодПозицииУказанийИТРоль,
        Позиции.КодДокумента,
        Позиции.КодРоли,
        Позиции.КодЛица,
        CASE	WHEN Позиции.КодЛица = 0 THEN '' 
	            ELSE ISNULL(ISNULL(ISNULL(NULLIF(Карточки.КраткоеНазваниеРус,''), NULLIF(Карточки.КраткоеНазваниеЛат,'')),Лица.Кличка), '#' + CONVERT(varchar,Позиции.КодЛица)) END НазваниеЛица,
        Позиции.Изменил,
        Позиции.Изменено       
FROM    (SELECT * FROM vwПозицииУказанийИТРоли (nolock) WHERE КодДокумента = @id) Позиции LEFT JOIN 
	    Справочники.dbo.vwЛица Лица (nolock) ON Позиции.КодЛица = Лица.КодЛица LEFT JOIN
	    Справочники.dbo.vwКарточкиЮрЛиц Карточки (nolock) ON Лица.КодЛица = Карточки.КодЛица AND CONVERT(date,GETDATE()) BETWEEN Карточки.От AND Карточки.До-1) T0
";

        /// <summary>
        ///     Подчиненный запрос без SELECT
        ///     Строка запроса: Получение позицию указаний роли по ID
        /// </summary>
        public const string SUBQUERY_ID_ПозицииУказанийИТРоли = @"
(SELECT	Позиции.КодПозицииУказанийИТРоль,
        Позиции.КодДокумента,
        Позиции.КодРоли,
        Позиции.КодЛица,
        CASE	WHEN Позиции.КодЛица = 0 THEN '' 
	            ELSE ISNULL(ISNULL(ISNULL(NULLIF(Карточки.КраткоеНазваниеРус,''), NULLIF(Карточки.КраткоеНазваниеЛат,'')),Лица.Кличка), '#' + CONVERT(varchar,Позиции.КодЛица)) END НазваниеЛица,
        Позиции.Изменил,
        Позиции.Изменено       
FROM    (SELECT * FROM vwПозицииУказанийИТРоли (nolock) WHERE КодПозицииУказанийИТРоль = @id) Позиции LEFT JOIN 
	    Справочники.dbo.vwЛица Лица (nolock) ON Позиции.КодЛица = Лица.КодЛица LEFT JOIN
	    Справочники.dbo.vwКарточкиЮрЛиц Карточки (nolock) ON Лица.КодЛица = Карточки.КодЛица AND CONVERT(date,GETDATE()) BETWEEN Карточки.От AND Карточки.До-1) T0
";

        /// <summary>
        ///     Подчиненный запрос без SELECT
        ///     Строка запроса: Получение позиций указаний типы по коду документа ID
        /// </summary>
        public const string SUBQUERY_ID_DOC_ПозицииУказанийИТТипы = @"
(SELECT	Позиции.КодПозицииУказанийИТТипЛица,
        Позиции.КодДокумента,
        Позиции.КодКаталога,
        ISNULL(Каталоги.Каталог,'') Каталог,        
        Позиции.КодТемыЛица,        
        ISNULL(ТемыЛиц.ТемаЛица,'') ТемаЛица,
        Позиции.Изменил,
        Позиции.Изменено       
FROM    (SELECT * FROM vwПозицииУказанийИТТипыЛиц (nolock) WHERE КодДокумента = @id) Позиции LEFT JOIN 
        Справочники.dbo.Каталоги Каталоги ON Позиции.КодКаталога = Каталоги.КодКаталога LEFT JOIN 
        Справочники.dbo.vwТемыЛиц ТемыЛиц ON Позиции.КодТемыЛица = ТемыЛиц.КодТемыЛица) T0
";

        /// <summary>
        ///     Подчиненный запрос без SELECT
        ///     Строка запроса: Получение позиции указаний типы по ID
        /// </summary>
        public const string SUBQUERY_ID_ПозицииУказанийИТТипы = @"
(SELECT	Позиции.КодПозицииУказанийИТТипЛица,
        Позиции.КодДокумента,
        Позиции.КодКаталога,
        ISNULL(Каталоги.Каталог,'<все каталоги>') Каталог,        
        Позиции.КодТемыЛица,        
        ISNULL(ТемыЛиц.ТемаЛица,'<все типы лиц>') ТемаЛица,
        Позиции.Изменил,
        Позиции.Изменено       
FROM    (SELECT * FROM vwПозицииУказанийИТТипыЛиц (nolock) WHERE КодПозицииУказанийИТТипЛица = @id) Позиции 
        LEFT JOIN Справочники.dbo.Каталоги Каталоги ON Позиции.КодКаталога = Каталоги.КодКаталога
        LEFT JOIN Справочники.dbo.vwТемыЛиц ТемыЛиц ON Позиции.КодТемыЛица = ТемыЛиц.КодТемыЛица) T0
";

        #endregion

        #endregion

        #region ДвиженияНаСкладах

        /// <summary>
        ///     Подчиненный запрос без SELECT
        ///     Строка запроса: Получение записи по коду движения на складе
        /// </summary>
        public const string SUBQUERY_ID_ДвиженияНаСкладах = @"
            (SELECT * FROM vwДвиженияНаСкладах (nolock) WHERE КодДвиженияНаСкладе=@id) T0
            ";

        /// <summary>
        ///     Подчиненный запрос без SELECT
        ///     Строка запроса: Получение движений на складе по коду документа
        /// </summary>
        public const string SUBQUERY_ID_DOC_ДвиженияНаСкладах = @"
            (SELECT * FROM vwДвиженияНаСкладах (nolock) WHERE КодДокумента=@id) T0
            ";

        /// <summary>
        ///     Строка запроса: Получение данных для списка движений на складах по коду документа
        /// </summary>
        public const string SELECT_ID_DOC_ДвиженияНаСкладах_GRID =
            @"
--Получение данных для списка движений на складах по коду документа
IF OBJECT_ID('tempdb..#ДвиженияНаСкладах') IS NOT NULL DROP TABLE #ДвиженияНаСкладах
CREATE TABLE #ДвиженияНаСкладах (КодДвиженияНаСкладе int PRIMARY KEY, КодОтправкиВагона int, ОтправкаВагона varchar(150), 
				КодРесурса int, РесурсРус varchar(200), 
				Количество float, КодЕдиницыИзмерения int,ЕдиницаРус nvarchar(10), Точность int NOT NULL DEFAULT(0),
				ЦенаБезНДС money, КодСтавкиНДС int, Величина100 float, 
				СуммаБезНДС money, СуммаНДС money, Всего money,
				КодСтраныПроисхождения int, СтранаПроисхождения varchar(80) NOT NULL DEFAULT(''),
				КодТаможеннойДекларации int, ТаможеннаяДекларация varchar(300) NOT NULL DEFAULT(''), Порядок int, Изменил int, Изменено datetime)

INSERT #ДвиженияНаСкладах(КодДвиженияНаСкладе, КодОтправкиВагона, ОтправкаВагона, КодРесурса, РесурсРус, Количество, КодЕдиницыИзмерения, КодСтавкиНДС,
			ЦенаБезНДС, СуммаБезНДС, СуммаНДС, Всего, КодСтраныПроисхождения, КодТаможеннойДекларации, Порядок, Изменил, Изменено)			
SELECT	КодДвиженияНаСкладе, КодОтправкиВагона, '', КодРесурса, РесурсРус, Количество, КодЕдиницыИзмерения, КодСтавкиНДС,
	    ЦенаБезНДС, СуммаБезНДС, СуммаНДС, Всего, КодСтраныПроисхождения, КодТаможеннойДекларации, Порядок, Изменил, Изменено	
FROM	vwДвиженияНаСкладах ДвиженияНаСкладах (nolock) 
WHERE	ДвиженияНаСкладах.КодДокумента = @КодДокумента		

UPDATE	X
SET	    ЕдиницаРус = ISNULL(ЕдиницыИзмерения.ЕдиницаРус,'')
FROM	#ДвиженияНаСкладах X LEFT JOIN Справочники.dbo.ЕдиницыИзмерения ЕдиницыИзмерения ON X.КодЕдиницыИзмерения = ЕдиницыИзмерения.КодЕдиницыИзмерения 

UPDATE	X
SET	    ОтправкаВагона = Вагоны.ОтправкаВагона
FROM	#ДвиженияНаСкладах X INNER JOIN vwОтправкаВагонов Вагоны (nolock) ON Вагоны.КодОтправкиВагона = X.КодОтправкиВагона

UPDATE	X
SET	    Величина100 = СтавкиНДС.Величина * 100
FROM	#ДвиженияНаСкладах X INNER JOIN Справочники.dbo.СтавкиНДС СтавкиНДС ON СтавкиНДС.КодСтавкиНДС = X.КодСтавкиНДС

UPDATE	X
SET	    СтранаПроисхождения = Территории.Территория
FROM	#ДвиженияНаСкладах X INNER JOIN Инвентаризация.dbo.Территории Территории ON Территории.КодТерритории = X.КодСтраныПроисхождения

UPDATE	X
SET	    ТаможеннаяДекларация = ISNULL(NULLIF(ТД.НазваниеДокумента,''), ТТД.ТипДокумента) + ' №' + ТД.НомерДокумента + ISNULL(' от ' + CONVERT(VARCHAR, ТД.ДатаДокумента,4),'')
FROM	#ДвиженияНаСкладах X INNER JOIN 
	    vwДокументы ТД (nolock) ON ТД.КодДокумента = X.КодТаможеннойДекларации INNER JOIN
	    ТипыДокументов ТТД ON ТД.КодТипаДокумента = ТТД.КодТипаДокумента

UPDATE	X
SET	    X.Точность = Ресурсы.Точность
FROM	#ДвиженияНаСкладах X INNER JOIN
	    Справочники.dbo.Ресурсы Ресурсы (nolock) ON X.КодРесурса = Ресурсы.КодРесурса AND X.КодЕдиницыИзмерения = Ресурсы.КодЕдиницыИзмерения
IF EXISTS(SELECT * FROM #ДвиженияНаСкладах WHERE Точность IS NULL)
	    UPDATE	X
	    SET	    X.Точность = ЕдиницыИзмерения.Точность
	    FROM	#ДвиженияНаСкладах X INNER JOIN
		        Справочники.dbo.ЕдиницыИзмеренияДополнительные ЕдиницыИзмерения ON X.КодРесурса = ЕдиницыИзмерения.КодРесурса AND X.КодЕдиницыИзмерения = ЕдиницыИзмерения.КодЕдиницыИзмерения

SELECT * FROM #ДвиженияНаСкладах ORDER BY Порядок

DROP TABLE #ДвиженияНаСкладах
            ";

        /// <summary>
        ///     Строка запроса: Получение данных для списка движений на складах по коду документа, сгруппированных по ресурсу
        /// </summary>
        public const string SELECT_ID_DOC_ДвиженияНаСкладах_GRID_ПоРесурсу =
            @"
--Получение данных для списка движений на складах по коду документа, сгруппированных по ресурсу
IF OBJECT_ID('tempdb..#ДвиженияНаСкладах') IS NOT NULL DROP TABLE #ДвиженияНаСкладах
CREATE TABLE #ДвиженияНаСкладах (КодРесурса int, РесурсРус varchar(200), 
				Количество float, КодЕдиницыИзмерения int,ЕдиницаРус nvarchar(10), Точность int NOT NULL DEFAULT(0),
				ЦенаБезНДС money, КодСтавкиНДС int, Величина100 float, 
				СуммаБезНДС money, СуммаНДС money, Всего money)

INSERT #ДвиженияНаСкладах (	КодРесурса, Количество, КодЕдиницыИзмерения, ЦенаБезНДС, КодСтавкиНДС, СуммаБезНДС, СуммаНДС, Всего)
SELECT	КодРесурса,	SUM(Количество) Количество, КодЕдиницыИзмерения, ЦенаБезНДС, КодСтавкиНДС, SUM(СуммаНДС) СуммаНДС, SUM(СуммаБезНДС) СуммаБезНДС, SUM(Всего) Всего
FROM	vwДвиженияНаСкладах ДвиженияНаСкладах (nolock) 	
WHERE	КодДокумента=@КодДокумента
GROUP BY КодРесурса, КодЕдиницыИзмерения, ЦенаБезНДС, КодСтавкиНДС

UPDATE	X
SET	    РесурсРус = Ресурсы.РесурсРус
FROM	#ДвиженияНаСкладах X INNER JOIN Справочники.dbo.Ресурсы Ресурсы ON X.КодРесурса = Ресурсы.КодРесурса 

UPDATE	X
SET	    ЕдиницаРус = ISNULL(ЕдиницыИзмерения.ЕдиницаРус,'')
FROM	#ДвиженияНаСкладах X LEFT JOIN Справочники.dbo.ЕдиницыИзмерения ЕдиницыИзмерения ON X.КодЕдиницыИзмерения = ЕдиницыИзмерения.КодЕдиницыИзмерения 

UPDATE	X
SET	    Величина100 = СтавкиНДС.Величина * 100
FROM	#ДвиженияНаСкладах X INNER JOIN Справочники.dbo.СтавкиНДС СтавкиНДС ON СтавкиНДС.КодСтавкиНДС = X.КодСтавкиНДС

UPDATE	X
SET	    X.Точность = Ресурсы.Точность
FROM	#ДвиженияНаСкладах X INNER JOIN
	    Справочники.dbo.Ресурсы Ресурсы (nolock) ON X.КодРесурса = Ресурсы.КодРесурса AND X.КодЕдиницыИзмерения = Ресурсы.КодЕдиницыИзмерения

IF EXISTS(SELECT * FROM #ДвиженияНаСкладах WHERE Точность IS NULL)
	UPDATE	X
	SET	    X.Точность = ЕдиницыИзмерения.Точность
	FROM	#ДвиженияНаСкладах X INNER JOIN
		    Справочники.dbo.ЕдиницыИзмеренияДополнительные ЕдиницыИзмерения (nolock) ON X.КодРесурса = ЕдиницыИзмерения.КодРесурса AND X.КодЕдиницыИзмерения = ЕдиницыИзмерения.КодЕдиницыИзмерения

SELECT * FROM #ДвиженияНаСкладах ORDER BY РесурсРус

DROP TABLE #ДвиженияНаСкладах         
";

        #endregion

        #region Оказанные услуги

        /// <summary>
        ///     Подчиненный запрос без SELECT
        ///     Строка запроса: Получение записи по коду оказанной услуги
        /// </summary>
        public const string SUBQUERY_ID_ОказанныеУслуги = @"
            (SELECT * FROM vwОказанныеУслуги (nolock) WHERE КодОказаннойУслуги=@id) T0
            ";

        /// <summary>
        ///     Подчиненный запрос без SELECT
        ///     Строка запроса: Получение оказанных услуг по коду документа
        /// </summary>
        public const string SUBQUERY_ID_DOC_ОказанныеУслуги = @"
            (SELECT * FROM vwОказанныеУслуги (nolock) WHERE КодДокумента=@id) T0
            ";

        /// <summary>
        ///     Строка запроса: Получение данных для списка услуг по коду документа
        /// </summary>
        public const string SELECT_ID_DOC_ОказанныеУслуги_GRID =
            @"
--Получение данных для списка услуг по коду документа
IF OBJECT_ID('tempdb..#ОказанныеУслуги') IS NOT NULL DROP TABLE #ОказанныеУслуги
CREATE TABLE #ОказанныеУслуги (КодОказаннойУслуги int PRIMARY KEY, GuidОказаннойУслуги uniqueidentifier,				
				КодРесурса int, РесурсРус varchar(300), РесурсЛат varchar(300), КодУчасткаОтправкиВагона int,
				Агент1 tinyint, Агент2 tinyint, КодДвиженияНаСкладе int, 
				Количество float, КодЕдиницыИзмерения int, ЕдиницаРус nvarchar(10), Точность int, Коэффициент float,
				ЦенаБезНДС money, КодСтавкиНДС int, Величина100 float, СуммаБезНДС money, СуммаНДС money, Всего money,
				Порядок int, Изменил int, Изменено datetime)
				
INSERT #ОказанныеУслуги(КодОказаннойУслуги, GuidОказаннойУслуги, Агент1, Агент2, КодДвиженияНаСкладе,
			КодРесурса, РесурсРус, РесурсЛат, КодУчасткаОтправкиВагона, Количество, КодЕдиницыИзмерения,
			Коэффициент, ЦенаБезНДС, СуммаБезНДС, КодСтавкиНДС, СуммаНДС, Всего, Порядок, Изменил, Изменено)
SELECT 	КодОказаннойУслуги, GuidОказаннойУслуги, Агент1, Агент2, КодДвиженияНаСкладе,
	    КодРесурса, РесурсРус, РесурсЛат, КодУчасткаОтправкиВагона, Количество, КодЕдиницыИзмерения,
	    Коэффициент, ЦенаБезНДС, СуммаБезНДС, КодСтавкиНДС, СуммаНДС, Всего, Порядок, Изменил, Изменено
FROM	vwОказанныеУслуги (nolock)
WHERE	КодДокумента = @КодДокумента

UPDATE	X
SET	    ЕдиницаРус = ISNULL(ЕдиницыИзмерения.ЕдиницаРус,'')
FROM	#ОказанныеУслуги X LEFT JOIN Справочники.dbo.ЕдиницыИзмерения ЕдиницыИзмерения ON X.КодЕдиницыИзмерения = ЕдиницыИзмерения.КодЕдиницыИзмерения 

UPDATE	X
SET	    Величина100 = СтавкиНДС.Величина * 100
FROM	#ОказанныеУслуги X INNER JOIN Справочники.dbo.СтавкиНДС СтавкиНДС ON СтавкиНДС.КодСтавкиНДС = X.КодСтавкиНДС


UPDATE	X
SET	    X.Точность = Ресурсы.Точность
FROM	#ОказанныеУслуги X INNER JOIN
	    Справочники.dbo.Ресурсы Ресурсы (nolock) ON X.КодРесурса = Ресурсы.КодРесурса AND X.КодЕдиницыИзмерения = Ресурсы.КодЕдиницыИзмерения
IF EXISTS(SELECT * FROM #ОказанныеУслуги WHERE Точность IS NULL)
	    UPDATE	X
	    SET		X.Точность = ЕдиницыИзмерения.Точность
	    FROM	#ОказанныеУслуги X INNER JOIN
		        Справочники.dbo.ЕдиницыИзмеренияДополнительные ЕдиницыИзмерения ON X.КодРесурса = ЕдиницыИзмерения.КодРесурса AND X.КодЕдиницыИзмерения = ЕдиницыИзмерения.КодЕдиницыИзмерения

UPDATE #ОказанныеУслуги SET Точность = 0 WHERE Точность IS NULL
     		
SELECT * FROM #ОказанныеУслуги ORDER BY Порядок     				
DROP TABLE #ОказанныеУслуги   	
            ";

        #endregion

        #region Таможенная декларация

        /// <summary>
        ///     Строка запроса: Получение документов
        /// </summary>
        public const string SELECT_ГТД = @"
SELECT T0.[КодДокумента], T0.[НомерДокумента], T0.[ДатаДокумента], T0.[Описание], T0.[Изменил], T0.[Изменено]
FROM vw_Д_ГТД T0 (nolock) ";

        /// <summary>
        ///     Строка запроса: Поиск документа по коду
        /// </summary>
        public static readonly string SELECT_ID_ГТД =
            string.Format(@"{0} WHERE T0.КодДокумента = @id", string.Format(SELECT_ГТД, ""));

        #endregion

        #region Ставка НДС

        /// <summary>
        ///     Строка запроса: Получить Ставки НДС
        /// </summary>
        public const string SELECT_ID_СтавкаНДС = @"SELECT 
                КодСтавкиНДС, СтавкаНДС, СтавкаНДСЛат, Величина, Приоритет, СпецНДС, КодТерритории, Действует
                FROM СтавкиНДС 
                WHERE КодСтавкиНДС = @Id
                ";


        /// <summary>
        ///     Строка запроса: Получить Ставки НДС
        /// </summary>
        public const string SELECT_СтавкиНДС = @"SELECT 
                КодСтавкиНДС, СтавкаНДС, СтавкаНДСЛат, Величина, Приоритет, СпецНДС, КодТерритории, Действует
                FROM СтавкиНДС 
                WHERE (КодТерритории IS NULL OR КодТерритории = @КодТерритории )
                ";

        #endregion

        #region Приложения Оценки Форм
        /// <summary>
        ///     Получение состояния Нравится - не нравится
        /// </summary>
        public const string SELECT_ОценкиИнтерфейса = @"
            SELECT Оценка FROM vwОценкиИнтерфейса WHERE КодИдентификатораОценки = @КодИдентификатораОценки";

        /// <summary>
        ///     Установка состояния Нравится - не нравится
        /// </summary>
        public const string INSERT_ОценкиИнтерфейса = @"
            INSERT vwОценкиИнтерфейса (КодИдентификатораОценки, ВерсияПО, Оценка) VALUES(@КодИдентификатораОценки, @ВерсияПО, @Оценка)";


        #endregion
    }
}