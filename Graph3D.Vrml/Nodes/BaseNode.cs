﻿using System.Collections.Generic;
using System.Diagnostics;
using Graph3D.Vrml.Fields;
using Graph3D.Vrml.Parser;

namespace Graph3D.Vrml.Nodes {
    public abstract class BaseNode {

        [DebuggerStepThrough]
        protected BaseNode() {
        }

        public string Name { get; set; }

        public BaseNode Parent { get; set; }

        private readonly Dictionary<string, Field> _exposedFields = new Dictionary<string, Field>();
        private readonly Dictionary<string, Field> _eventIns = new Dictionary<string, Field>();
        private readonly Dictionary<string, Field> _eventOuts = new Dictionary<string, Field>();

        protected void AddField(string fieldName, Field field) {
            //TODO: another dictionary.
            _exposedFields[fieldName] = field;
        }

        protected void AddExposedField(string exposedFieldName, Field field) {
            _exposedFields[exposedFieldName] = field;
        }

        protected void AddEventIn(string eventInName, Field field) {
            _eventIns[eventInName] = field;
        }

        protected void AddEventOut(string eventOutName, Field field) {
            _eventOuts[eventOutName] = field;
        }

        public IDictionary<string, Field> AllFields => _exposedFields.AsReadOnly();

        public Field GetExposedField(string exposedFieldName) {
            if (_exposedFields.TryGetValue(exposedFieldName, out Field field)) {
                return field;
            }
            throw new InvalidExposedFieldException($"'{exposedFieldName}' exposed field doesn't exist in node of {GetType().Name} type");
        }

        public Field GetField(string fieldName) {
            if (_exposedFields.TryGetValue(fieldName, out Field res)) {
                return res;
            }
            throw new InvalidExposedFieldException($"'{fieldName}' field doesn't exist in node of {GetType().Name} type");
        }

        public Field GetEventIn(string eventInName) {
            if (_eventIns.TryGetValue(eventInName, out Field res)) {
                return res;
            }
            throw new InvalidEventInException($"'{eventInName}' event in field doesn't exist in node of {GetType().Name} type");
        }

        public Field GetEventOut(string eventOutName) {
            if (_eventOuts.TryGetValue(eventOutName, out Field res)) return res;
            throw new InvalidEventOutException($"'{eventOutName}' event out field doesn't exist in node of {GetType().Name} type");
        }

        protected abstract BaseNode CreateInstance();

        public abstract void AcceptVisitor(INodeVisitor visitor);

        public virtual BaseNode Clone() {
            var clone = CreateInstance();
            foreach (var key in _exposedFields.Keys) {
                Field field = _exposedFields[key];
                clone._exposedFields[key] = field.Clone();
            }
            foreach (var key in _eventIns.Keys) {
                Field field = _eventIns[key];
                clone._eventIns[key] = field.Clone();
            }
            foreach (var key in _eventOuts.Keys) {
                Field field = _eventOuts[key];
                clone._eventOuts[key] = field.Clone();
            }
            clone.Name = Name;
            return clone;
        }

        public override string ToString() {
            string fieldsStr = "";
            foreach (string key in _eventIns.Keys) {
                if (!string.IsNullOrEmpty(fieldsStr)) fieldsStr += ", \r\n";
                fieldsStr += key + ": " + _eventIns[key].ToString();
            }
            foreach (string key in _eventOuts.Keys) {
                if (!string.IsNullOrEmpty(fieldsStr)) fieldsStr += ", \r\n";
                fieldsStr += key + ": " + _eventOuts[key].ToString();
            }
            foreach (string key in _exposedFields.Keys) {
                if (!string.IsNullOrEmpty(fieldsStr)) fieldsStr += ", \r\n";
                fieldsStr += key + ": " + _exposedFields[key].ToString();
            }
            if (!string.IsNullOrEmpty(fieldsStr)) fieldsStr += "\r\n";
            return string.Format("{0}: {{\r\n{1}}}", GetType().Name, fieldsStr);
        }
    }
}
