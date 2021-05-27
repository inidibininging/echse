using System;
using System.Collections.Generic;

namespace Echse.Domain
{
    public interface IEchseContext
    {
        /// <summary>
        /// Tick used in the wait function of the echse language
        /// </summary>
        TimeSpan LanguageTick { get; }
        /// <summary>
        /// Interface to all variables of the language
        /// </summary>
        IEnumerable<TagVariable> Variables { get; }
        
        /// <summary>
        /// Injects a variable to the language
        /// </summary>
        /// <param name="variable"></param>
        void AddVariable(TagVariable variable);
        
        /// <summary>
        /// Removes a tag from the language
        /// </summary>
        /// <param name="tagName"></param>
        void RemoveTag(string tagName);

    }
}