﻿namespace AngleSharp.Parser.Html
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// The abstract base class of any HTML token.
    /// </summary>
    [DebuggerStepThrough]
    abstract class HtmlToken
    {
        #region Factory

        /// <summary>
        /// Gets the end of file token.
        /// </summary>
        public static HtmlEndOfFileToken EOF
        {
            get { return eof ?? (eof = new HtmlEndOfFileToken()); }
        }

        /// <summary>
        /// Creates a new HTML character token based on the given characters.
        /// </summary>
        /// <param name="characters">The characters to contain.</param>
        /// <returns>The generated token.</returns>
        [DebuggerStepThrough]
        public static HtmlCharacterToken Character(String characters)
        {
            return new HtmlCharacterToken(characters);
        }

        /// <summary>
        /// Creates a new HTML comment token based on the given string.
        /// </summary>
        /// <param name="comment">The comment to contain.</param>
        /// <returns>The generated token.</returns>
        [DebuggerStepThrough]
        public static HtmlCommentToken Comment(String comment)
        {
            return new HtmlCommentToken(comment);
        }

        /// <summary>
        /// Creates a new HTML doctype token.
        /// </summary>
        /// <param name="quirksmode">Determines if quirksmode will be forced.</param>
        /// <returns>The generated token.</returns>
        [DebuggerStepThrough]
        public static HtmlDoctypeToken Doctype(Boolean quirksmode)
        {
            return new HtmlDoctypeToken(quirksmode);
        }

        /// <summary>
        /// Creates a new opening HtmlTagToken.
        /// </summary>
        /// <returns>The new HTML tag token.</returns>
        [DebuggerStepThrough]
        public static HtmlTagToken OpenTag()
        {
            return new HtmlTagToken { _type = HtmlTokenType.StartTag };
        }

        /// <summary>
        /// Creates a new closing HtmlTagToken.
        /// </summary>
        /// <returns>The new HTML tag token.</returns>
        [DebuggerStepThrough]
        public static HtmlTagToken CloseTag()
        {
            return new HtmlTagToken { _type = HtmlTokenType.EndTag };
        }

        /// <summary>
        /// Creates a new opening HtmlTagToken for the given name.
        /// </summary>
        /// <param name="name">The name of the tag.</param>
        /// <returns>The new HTML tag token.</returns>
        [DebuggerStepThrough]
        public static HtmlTagToken OpenTag(String name)
        {
            return new HtmlTagToken(name) { _type = HtmlTokenType.StartTag };
        }

        /// <summary>
        /// Creates a new closing HtmlTagToken for the given name.
        /// </summary>
        /// <param name="name">The name of the tag.</param>
        /// <returns>The new HTML tag token.</returns>
        [DebuggerStepThrough]
        public static HtmlTagToken CloseTag(String name)
        {
            return new HtmlTagToken(name) { _type = HtmlTokenType.EndTag };
        }

        #endregion

        #region Fields

        static HtmlEndOfFileToken eof;
        protected HtmlTokenType _type;
        protected String _name;

        #endregion

        #region Properties

        /// <summary>
        /// Gets if the character data is empty (null or length equal to zero).
        /// </summary>
        /// <returns>True if the character data is actually NULL or empty.</returns>
        public Boolean IsEmpty
        {
            get { return String.IsNullOrEmpty(_name); }
        }

        /// <summary>
        /// Gets if the character data contains actually a non-space character.
        /// </summary>
        /// <returns>True if the character data contains space character.</returns>
        public Boolean HasContent
        {
            get
            {
                for (int i = 0; i < _name.Length; i++)
                {
                    if (!_name[i].IsSpaceCharacter())
                        return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Gets or sets the name of a tag token.
        /// </summary>
        public String Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Gets the data of the comment or character token.
        /// </summary>
        public String Data
        {
            get { return _name; }
        }

        /// <summary>
        /// Gets if the token can be used with IsHtmlTIP properties.
        /// </summary>
        public virtual Boolean IsHtmlCompatible
        {
            get { return false; }
        }

        /// <summary>
        /// Gets if the given token is a SVG root start tag.
        /// </summary>
        public Boolean IsSvg
        {
            get { return IsStartTag(Tags.Svg); }
        }

        /// <summary>
        /// Gets if the token is an end-of-file token.
        /// </summary>
        public Boolean IsEof
        {
            get { return _type == HtmlTokenType.EOF; }
        }

        /// <summary>
        /// Gets if the token can be used with IsMathMLTIP properties.
        /// </summary>
        public virtual Boolean IsMathCompatible
        {
            get { return false; }
        }

        /// <summary>
        /// Gets the type of the token.
        /// </summary>
        public HtmlTokenType Type
        {
            get { return _type; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Removes all ignorable characters from the beginning.
        /// </summary>
        /// <returns>The trimmed characters.</returns>
        public String TrimStart()
        {
            var i = 0;

            for (i = 0; i < _name.Length; i++)
            {
                if (!_name[i].IsSpaceCharacter())
                    break;
            }

            var t = _name.Substring(0, i);
            _name = _name.Substring(i);
            return t;
        }

        /// <summary>
        /// Removes the a new line in the beginning, if any.
        /// </summary>
        public void RemoveNewLine()
        {
            if (!String.IsNullOrEmpty(_name) && _name[0] == Specification.LineFeed)
                _name = _name.Substring(1);
        }

        /// <summary>
        /// Converts the current token to a tag token.
        /// </summary>
        /// <returns>The tag token instance.</returns>
        public HtmlTagToken AsTag()
        {
            return (HtmlTagToken)this;
        }

        /// <summary>
        /// Finds out if the current token is a start or end tag token with the given name.
        /// </summary>
        /// <param name="name">The name of the tag.</param>
        /// <returns>True if the token is indeed a start or end tag token with the given name, otherwise false.</returns>
        public Boolean IsTag(String name)
        {
            return (_type == HtmlTokenType.StartTag || _type == HtmlTokenType.EndTag) && _name.Equals(name);
        }

        /// <summary>
        /// Finds out if the current token is an end tag token with the given name.
        /// </summary>
        /// <param name="name">The name of the tag.</param>
        /// <returns>True if the token is indeed an end tag token with the given name, otherwise false.</returns>
        public Boolean IsEndTag(String name)
        {
            return _type == HtmlTokenType.EndTag && _name.Equals(name);
        }

        /// <summary>
        /// Finds out if the current token is an end tag token with a name that is different than the given one.
        /// </summary>
        /// <param name="name">The name of the tag.</param>
        /// <returns>True if the token is indeed an end tag token and does NOT have the given name, otherwise false.</returns>
        public Boolean IsEndTagInv(String name)
        {
            return _type == HtmlTokenType.EndTag && !_name.Equals(name);
        }

        /// <summary>
        /// Finds out if the current token is an end tag token with one of the given name(s).
        /// </summary>
        /// <param name="nameA">The name of the tag.</param>
        /// <param name="nameB">The name of the other tag.</param>
        /// <returns>True if the token is indeed an end tag token with the given name, otherwise false.</returns>
        public Boolean IsEndTag(String nameA, String nameB)
        {
            return _type == HtmlTokenType.EndTag && (_name.Equals(nameA) || _name.Equals(nameB));
        }

        /// <summary>
        /// Finds out if the current token is an end tag token with one of the given name(s).
        /// </summary>
        /// <param name="nameA">The name of the tag.</param>
        /// <param name="nameB">The name of another tag.</param>
        /// <param name="nameC">The name of the other tag.</param>
        /// <returns>True if the token is indeed an end tag token with the given name, otherwise false.</returns>
        public Boolean IsEndTag(String nameA, String nameB, String nameC)
        {
            return _type == HtmlTokenType.EndTag && (_name.Equals(nameA) || _name.Equals(nameB) || _name.Equals(nameC));
        }

        /// <summary>
        /// Finds out if the current token is an end tag token with a name that is different than the given one.
        /// </summary>
        /// <param name="nameA">The name of the tag.</param>
        /// <param name="nameB">The name of another tag.</param>
        /// <param name="nameC">The name of the other tag.</param>
        /// <returns>True if the token is indeed an end tag token and does NOT have the given name, otherwise false.</returns>
        public Boolean IsEndTagInv(String nameA, String nameB, String nameC)
        {
            return _type == HtmlTokenType.EndTag && !_name.Equals(nameA) && !_name.Equals(nameB) && !_name.Equals(nameC);
        }

        /// <summary>
        /// Finds out if the current token is an end tag token with one of the given name(s).
        /// </summary>
        /// <param name="nameA">The name of the tag.</param>
        /// <param name="nameB">The name of another tag.</param>
        /// <param name="nameC">The name of the other tag.</param>
        /// <param name="nameD">The name of yet another tag.</param>
        /// <returns>True if the token is indeed an end tag token with the given name, otherwise false.</returns>
        public Boolean IsEndTag(String nameA, String nameB, String nameC, String nameD)
        {
            return _type == HtmlTokenType.EndTag && (_name.Equals(nameA) || _name.Equals(nameB) || _name.Equals(nameC) || _name.Equals(nameD));
        }

        /// <summary>
        /// Finds out if the current token is an end tag token with a name that is different than the given one.
        /// </summary>
        /// <param name="nameA">The name of the tag.</param>
        /// <param name="nameB">The name of another tag.</param>
        /// <param name="nameC">The name of the other tag.</param>
        /// <param name="nameD">The name of yet another tag.</param>
        /// <returns>True if the token is indeed an end tag token and does NOT have the given name, otherwise false.</returns>
        public Boolean IsEndTagInv(String nameA, String nameB, String nameC, String nameD)
        {
            return _type == HtmlTokenType.EndTag && !_name.Equals(nameA) && !_name.Equals(nameB) && !_name.Equals(nameC) && !_name.Equals(nameD);
        }

        /// <summary>
        /// Finds out if the current token is a start tag token with the given name.
        /// </summary>
        /// <param name="name">The name of the tag.</param>
        /// <returns>True if the token is indeed a start tag token with the given name, otherwise false.</returns>
        public Boolean IsStartTag(String name)
        {
            return _type == HtmlTokenType.StartTag && _name.Equals(name);
        }

        /// <summary>
        /// Finds out if the current token is a start tag token with one of the given name(s).
        /// </summary>
        /// <param name="nameA">The name of the tag.</param>
        /// <param name="nameB">The name of the other tag.</param>
        /// <returns>True if the token is indeed a start tag token with the given name, otherwise false.</returns>
        public Boolean IsStartTag(String nameA, String nameB)
        {
            return _type == HtmlTokenType.StartTag && (_name.Equals(nameA) || _name.Equals(nameB));
        }

        /// <summary>
        /// Finds out if the current token is a start tag token with one of the given name(s).
        /// </summary>
        /// <param name="nameA">The name of the tag.</param>
        /// <param name="nameB">The name of another tag.</param>
        /// <param name="nameC">The name of the other tag.</param>
        /// <returns>True if the token is indeed a start tag token with the given name, otherwise false.</returns>
        public Boolean IsStartTag(String nameA, String nameB, String nameC)
        {
            return _type == HtmlTokenType.StartTag && (_name.Equals(nameA) || _name.Equals(nameB) || _name.Equals(nameC));
        }

        /// <summary>
        /// Finds out if the current token is a start tag token with one of the given name(s).
        /// </summary>
        /// <param name="nameA">The name of the tag.</param>
        /// <param name="nameB">The name of another tag.</param>
        /// <param name="nameC">The name of the other tag.</param>
        /// <param name="nameD">The name of yet another tag.</param>
        /// <returns>True if the token is indeed a start tag token with the given name, otherwise false.</returns>
        public Boolean IsStartTag(String nameA, String nameB, String nameC, String nameD)
        {
            return _type == HtmlTokenType.StartTag && (_name.Equals(nameA) || _name.Equals(nameB) || _name.Equals(nameC) || _name.Equals(nameD));
        }

        /// <summary>
        /// Finds out if the current token is a start tag token with one of the given name(s).
        /// </summary>
        /// <param name="nameA">The name of the tag.</param>
        /// <param name="nameB">The name of another tag.</param>
        /// <param name="nameC">The name of the other tag.</param>
        /// <param name="nameD">The name of yet another tag.</param>
        /// <param name="nameE">One more name.</param>
        /// <param name="nameF">And another name.</param>
        /// <returns>True if the token is indeed a start tag token with the given name, otherwise false.</returns>
        public Boolean IsStartTag(String nameA, String nameB, String nameC, String nameD, String nameE, String nameF)
        {
            return _type == HtmlTokenType.StartTag && (_name.Equals(nameA) || _name.Equals(nameB) || _name.Equals(nameC) || _name.Equals(nameD) || _name.Equals(nameE) || _name.Equals(nameF));
        }

        /// <summary>
        /// Finds out if the current token is a start tag token with one of the given name(s).
        /// </summary>
        /// <param name="nameA">The name of the tag.</param>
        /// <param name="nameB">The name of another tag.</param>
        /// <param name="nameC">The name of the other tag.</param>
        /// <param name="nameD">The name of yet another tag.</param>
        /// <param name="nameE">One more name.</param>
        /// <param name="nameF">And another name.</param>
        /// <param name="nameG">The name of yet another tag.</param>
        /// <param name="nameH">One more name.</param>
        /// <param name="nameJ">And another name.</param>
        /// <returns>True if the token is indeed a start tag token with the given name, otherwise false.</returns>
        public Boolean IsStartTag(String nameA, String nameB, String nameC, String nameD, String nameE, String nameF, String nameG, String nameH, String nameJ)
        {
            return _type == HtmlTokenType.StartTag && (_name.Equals(nameA) || _name.Equals(nameB) || _name.Equals(nameC) || _name.Equals(nameD) || _name.Equals(nameE) || _name.Equals(nameF) || _name.Equals(nameG) || _name.Equals(nameH) || _name.Equals(nameJ));
        }

        #endregion
    }
}
