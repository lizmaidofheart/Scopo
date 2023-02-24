using System;

// this is a class for use to make a dictionary of string and BrainState serializable. this solution is taken from http://answers.unity.com/answers/809221/view.html

[Serializable] public class DictionaryOfStringAndBrainState : SerializableDictionary<string, BrainState> { }