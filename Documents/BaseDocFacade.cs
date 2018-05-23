using System;
using System.Diagnostics;
using Kesco.Lib.BaseExtention;
using Kesco.Lib.BaseExtention.BindModels;
using Kesco.Lib.BaseExtention.Enums.Docs;

namespace Kesco.Lib.Entities.Documents
{
    
    /// <summary>
    /// Класс абстрагирует работу с документом и полем документом(паттерн фасад), так же реализует возможность связывания данных
    /// </summary>
    /// <remarks>
    ///  Если у поля нет маппинга значит значения сохраняются в СвязиДокументов, эту функцию берет на себя этот класс
    ///  на данный момент(30.08.2017) этот класс использует DocField и инкапсулирует всю логику работы с BaseDocFacade и програмисту незачем самостоятельно реализовывать логику
    /// </remarks>
    [DebuggerDisplay("Value = {Value}, DocField = {_field.DocFieldId}")]
    public class BaseDocFacade : IBinderValue<string>
    {
        /// <summary>
        ///  Конструктор
        /// </summary>
        /// <param name="doc">Документ</param>
        /// <param name="field">Поле документа</param>
        /// <param name="behavior">Поведение по умолчанию при установке Set значения</param>
        public BaseDocFacade(Document doc, DocField field, BaseSetBehavior behavior = BaseSetBehavior.SetBaseDoc)
        {
            _document = doc;
            _field = field;

            // поведение set по умолчанию
            ChangeBehavior(behavior);
        }

        private readonly Document _document;
        private readonly DocField _field;
        private  Action<string> SetAction;

        /// <summary>
        ///  Установка базового документа
        /// </summary>
        private void RemoveAllAndAddDoc(string s)
        {
            _document.RemoveAllBaseDocs(_field.DocFieldId);
            _document.AddBaseDoc(s, _field.DocFieldId);
        }

        /// <summary>
        ///  Установка базового документа 
        /// </summary>
        /// <remarks>по умолчанию</remarks>
        private void SetBaseDoc(string s)
        {
            _document.SetBaseDoc(_field.DocFieldId, s.ToInt());
        }

        /// <summary>
        ///  Изменить поведение при сохранении значения
        /// </summary>
        public void ChangeBehavior(BaseSetBehavior behavior)
        {
            switch (behavior)
            {
                case BaseSetBehavior.SetBaseDoc:
                    SetAction = SetBaseDoc;
                    break;
                case BaseSetBehavior.RemoveAllAndAddDoc:
                    SetAction = RemoveAllAndAddDoc;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Для поведения " + behavior + " не уставнолен делегат");
            }
        }

        /// <summary>
        ///  Предыдущее значение
        /// </summary>
        private string _oldValue;

        /// <summary>
        ///  Значение
        /// </summary>
        public string Value {
            get { return _document.GetBaseDoc(_field.DocFieldId); }
            set
            {
                if (value != _oldValue)
                {
                    SetAction(value);
                    ValueChangedEvent_Invoke(value, _oldValue);
                }

                _oldValue = value;
            } 
        }

        /// <summary>
        ///  Событие изменения значения
        /// </summary>
        public event ValueChangedEventHandler ValueChangedEvent;

        /// <summary>
        ///  Выполяняет действия события ValueChangedEvent
        /// </summary>
        public void ValueChangedEvent_Invoke(string newVal, string oldVal)
        {
            var handler = ValueChangedEvent;

            if (handler != null)
                handler(this, new ValueChangedEventArgs(newVal, oldVal));
        }
    }
}
