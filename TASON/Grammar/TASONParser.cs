//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.13.2
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from TASON.g4 by ANTLR 4.13.2

// Unreachable code detected
#pragma warning disable 0162
// The variable '...' is assigned but its value is never used
#pragma warning disable 0219
// Missing XML comment for publicly visible type or member '...'
#pragma warning disable 1591
// Ambiguous reference in cref attribute
#pragma warning disable 419

namespace TASON.Grammar {
using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using DFA = Antlr4.Runtime.Dfa.DFA;

[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.13.2")]
[System.CLSCompliant(false)]
public partial class TASONParser : Parser {
	protected static DFA[] decisionToDFA;
	protected static PredictionContextCache sharedContextCache = new PredictionContextCache();
	public const int
		T__0=1, T__1=2, T__2=3, T__3=4, T__4=5, T__5=6, T__6=7, T__7=8, T__8=9, 
		T__9=10, BOOLEAN_TRUE=11, BOOLEAN_FALSE=12, NULL=13, TYPE_NAME=14, IDENTIFIER=15, 
		WS=16, SINGLE_LINE_COMMENT=17, MULTI_LINE_COMMENT=18, STRING=19, SYMBOL=20, 
		NUMBER=21, INVALID_CHAR=22;
	public const int
		RULE_start = 0, RULE_value = 1, RULE_boolean = 2, RULE_typeInstance = 3, 
		RULE_object = 4, RULE_pair = 5, RULE_key = 6, RULE_array = 7, RULE_number = 8;
	public static readonly string[] ruleNames = {
		"start", "value", "boolean", "typeInstance", "object", "pair", "key", 
		"array", "number"
	};

	private static readonly string[] _LiteralNames = {
		null, "'('", "')'", "'{'", "','", "'}'", "':'", "'['", "']'", "'NaN'", 
		"'Infinity'", "'true'", "'false'", "'null'"
	};
	private static readonly string[] _SymbolicNames = {
		null, null, null, null, null, null, null, null, null, null, null, "BOOLEAN_TRUE", 
		"BOOLEAN_FALSE", "NULL", "TYPE_NAME", "IDENTIFIER", "WS", "SINGLE_LINE_COMMENT", 
		"MULTI_LINE_COMMENT", "STRING", "SYMBOL", "NUMBER", "INVALID_CHAR"
	};
	public static readonly IVocabulary DefaultVocabulary = new Vocabulary(_LiteralNames, _SymbolicNames);

	[NotNull]
	public override IVocabulary Vocabulary
	{
		get
		{
			return DefaultVocabulary;
		}
	}

	public override string GrammarFileName { get { return "TASON.g4"; } }

	public override string[] RuleNames { get { return ruleNames; } }

	public override int[] SerializedAtn { get { return _serializedATN; } }

	static TASONParser() {
		decisionToDFA = new DFA[_ATN.NumberOfDecisions];
		for (int i = 0; i < _ATN.NumberOfDecisions; i++) {
			decisionToDFA[i] = new DFA(_ATN.GetDecisionState(i), i);
		}
	}

		public TASONParser(ITokenStream input) : this(input, Console.Out, Console.Error) { }

		public TASONParser(ITokenStream input, TextWriter output, TextWriter errorOutput)
		: base(input, output, errorOutput)
	{
		Interpreter = new ParserATNSimulator(this, _ATN, decisionToDFA, sharedContextCache);
	}

	public partial class StartContext : ParserRuleContext {
		[System.Diagnostics.DebuggerNonUserCode] public ValueContext value() {
			return GetRuleContext<ValueContext>(0);
		}
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode Eof() { return GetToken(TASONParser.Eof, 0); }
		public StartContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_start; } }
		[System.Diagnostics.DebuggerNonUserCode]
		public override void EnterRule(IParseTreeListener listener) {
			ITASONListener typedListener = listener as ITASONListener;
			if (typedListener != null) typedListener.EnterStart(this);
		}
		[System.Diagnostics.DebuggerNonUserCode]
		public override void ExitRule(IParseTreeListener listener) {
			ITASONListener typedListener = listener as ITASONListener;
			if (typedListener != null) typedListener.ExitStart(this);
		}
	}

	[RuleVersion(0)]
	public StartContext start() {
		StartContext _localctx = new StartContext(Context, State);
		EnterRule(_localctx, 0, RULE_start);
		try {
			EnterOuterAlt(_localctx, 1);
			{
			State = 18;
			value();
			State = 19;
			Match(Eof);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	public partial class ValueContext : ParserRuleContext {
		public ValueContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_value; } }
	 
		public ValueContext() { }
		public virtual void CopyFrom(ValueContext context) {
			base.CopyFrom(context);
		}
	}
	public partial class ObjectValueContext : ValueContext {
		[System.Diagnostics.DebuggerNonUserCode] public ObjectContext @object() {
			return GetRuleContext<ObjectContext>(0);
		}
		public ObjectValueContext(ValueContext context) { CopyFrom(context); }
		[System.Diagnostics.DebuggerNonUserCode]
		public override void EnterRule(IParseTreeListener listener) {
			ITASONListener typedListener = listener as ITASONListener;
			if (typedListener != null) typedListener.EnterObjectValue(this);
		}
		[System.Diagnostics.DebuggerNonUserCode]
		public override void ExitRule(IParseTreeListener listener) {
			ITASONListener typedListener = listener as ITASONListener;
			if (typedListener != null) typedListener.ExitObjectValue(this);
		}
	}
	public partial class NullValueContext : ValueContext {
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode NULL() { return GetToken(TASONParser.NULL, 0); }
		public NullValueContext(ValueContext context) { CopyFrom(context); }
		[System.Diagnostics.DebuggerNonUserCode]
		public override void EnterRule(IParseTreeListener listener) {
			ITASONListener typedListener = listener as ITASONListener;
			if (typedListener != null) typedListener.EnterNullValue(this);
		}
		[System.Diagnostics.DebuggerNonUserCode]
		public override void ExitRule(IParseTreeListener listener) {
			ITASONListener typedListener = listener as ITASONListener;
			if (typedListener != null) typedListener.ExitNullValue(this);
		}
	}
	public partial class NumberValueContext : ValueContext {
		[System.Diagnostics.DebuggerNonUserCode] public NumberContext number() {
			return GetRuleContext<NumberContext>(0);
		}
		public NumberValueContext(ValueContext context) { CopyFrom(context); }
		[System.Diagnostics.DebuggerNonUserCode]
		public override void EnterRule(IParseTreeListener listener) {
			ITASONListener typedListener = listener as ITASONListener;
			if (typedListener != null) typedListener.EnterNumberValue(this);
		}
		[System.Diagnostics.DebuggerNonUserCode]
		public override void ExitRule(IParseTreeListener listener) {
			ITASONListener typedListener = listener as ITASONListener;
			if (typedListener != null) typedListener.ExitNumberValue(this);
		}
	}
	public partial class BooleanValueContext : ValueContext {
		[System.Diagnostics.DebuggerNonUserCode] public BooleanContext boolean() {
			return GetRuleContext<BooleanContext>(0);
		}
		public BooleanValueContext(ValueContext context) { CopyFrom(context); }
		[System.Diagnostics.DebuggerNonUserCode]
		public override void EnterRule(IParseTreeListener listener) {
			ITASONListener typedListener = listener as ITASONListener;
			if (typedListener != null) typedListener.EnterBooleanValue(this);
		}
		[System.Diagnostics.DebuggerNonUserCode]
		public override void ExitRule(IParseTreeListener listener) {
			ITASONListener typedListener = listener as ITASONListener;
			if (typedListener != null) typedListener.ExitBooleanValue(this);
		}
	}
	public partial class StringValueContext : ValueContext {
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode STRING() { return GetToken(TASONParser.STRING, 0); }
		public StringValueContext(ValueContext context) { CopyFrom(context); }
		[System.Diagnostics.DebuggerNonUserCode]
		public override void EnterRule(IParseTreeListener listener) {
			ITASONListener typedListener = listener as ITASONListener;
			if (typedListener != null) typedListener.EnterStringValue(this);
		}
		[System.Diagnostics.DebuggerNonUserCode]
		public override void ExitRule(IParseTreeListener listener) {
			ITASONListener typedListener = listener as ITASONListener;
			if (typedListener != null) typedListener.ExitStringValue(this);
		}
	}
	public partial class TypeInstanceValueContext : ValueContext {
		[System.Diagnostics.DebuggerNonUserCode] public TypeInstanceContext typeInstance() {
			return GetRuleContext<TypeInstanceContext>(0);
		}
		public TypeInstanceValueContext(ValueContext context) { CopyFrom(context); }
		[System.Diagnostics.DebuggerNonUserCode]
		public override void EnterRule(IParseTreeListener listener) {
			ITASONListener typedListener = listener as ITASONListener;
			if (typedListener != null) typedListener.EnterTypeInstanceValue(this);
		}
		[System.Diagnostics.DebuggerNonUserCode]
		public override void ExitRule(IParseTreeListener listener) {
			ITASONListener typedListener = listener as ITASONListener;
			if (typedListener != null) typedListener.ExitTypeInstanceValue(this);
		}
	}
	public partial class ArrayValueContext : ValueContext {
		[System.Diagnostics.DebuggerNonUserCode] public ArrayContext array() {
			return GetRuleContext<ArrayContext>(0);
		}
		public ArrayValueContext(ValueContext context) { CopyFrom(context); }
		[System.Diagnostics.DebuggerNonUserCode]
		public override void EnterRule(IParseTreeListener listener) {
			ITASONListener typedListener = listener as ITASONListener;
			if (typedListener != null) typedListener.EnterArrayValue(this);
		}
		[System.Diagnostics.DebuggerNonUserCode]
		public override void ExitRule(IParseTreeListener listener) {
			ITASONListener typedListener = listener as ITASONListener;
			if (typedListener != null) typedListener.ExitArrayValue(this);
		}
	}

	[RuleVersion(0)]
	public ValueContext value() {
		ValueContext _localctx = new ValueContext(Context, State);
		EnterRule(_localctx, 2, RULE_value);
		try {
			State = 28;
			ErrorHandler.Sync(this);
			switch (TokenStream.LA(1)) {
			case T__2:
				_localctx = new ObjectValueContext(_localctx);
				EnterOuterAlt(_localctx, 1);
				{
				State = 21;
				@object();
				}
				break;
			case T__6:
				_localctx = new ArrayValueContext(_localctx);
				EnterOuterAlt(_localctx, 2);
				{
				State = 22;
				array();
				}
				break;
			case STRING:
				_localctx = new StringValueContext(_localctx);
				EnterOuterAlt(_localctx, 3);
				{
				State = 23;
				Match(STRING);
				}
				break;
			case T__8:
			case T__9:
			case SYMBOL:
			case NUMBER:
				_localctx = new NumberValueContext(_localctx);
				EnterOuterAlt(_localctx, 4);
				{
				State = 24;
				number();
				}
				break;
			case BOOLEAN_TRUE:
			case BOOLEAN_FALSE:
				_localctx = new BooleanValueContext(_localctx);
				EnterOuterAlt(_localctx, 5);
				{
				State = 25;
				boolean();
				}
				break;
			case NULL:
				_localctx = new NullValueContext(_localctx);
				EnterOuterAlt(_localctx, 6);
				{
				State = 26;
				Match(NULL);
				}
				break;
			case TYPE_NAME:
				_localctx = new TypeInstanceValueContext(_localctx);
				EnterOuterAlt(_localctx, 7);
				{
				State = 27;
				typeInstance();
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	public partial class BooleanContext : ParserRuleContext {
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode BOOLEAN_TRUE() { return GetToken(TASONParser.BOOLEAN_TRUE, 0); }
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode BOOLEAN_FALSE() { return GetToken(TASONParser.BOOLEAN_FALSE, 0); }
		public BooleanContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_boolean; } }
		[System.Diagnostics.DebuggerNonUserCode]
		public override void EnterRule(IParseTreeListener listener) {
			ITASONListener typedListener = listener as ITASONListener;
			if (typedListener != null) typedListener.EnterBoolean(this);
		}
		[System.Diagnostics.DebuggerNonUserCode]
		public override void ExitRule(IParseTreeListener listener) {
			ITASONListener typedListener = listener as ITASONListener;
			if (typedListener != null) typedListener.ExitBoolean(this);
		}
	}

	[RuleVersion(0)]
	public BooleanContext boolean() {
		BooleanContext _localctx = new BooleanContext(Context, State);
		EnterRule(_localctx, 4, RULE_boolean);
		int _la;
		try {
			EnterOuterAlt(_localctx, 1);
			{
			State = 30;
			_la = TokenStream.LA(1);
			if ( !(_la==BOOLEAN_TRUE || _la==BOOLEAN_FALSE) ) {
			ErrorHandler.RecoverInline(this);
			}
			else {
				ErrorHandler.ReportMatch(this);
			    Consume();
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	public partial class TypeInstanceContext : ParserRuleContext {
		public TypeInstanceContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_typeInstance; } }
	 
		public TypeInstanceContext() { }
		public virtual void CopyFrom(TypeInstanceContext context) {
			base.CopyFrom(context);
		}
	}
	public partial class ScalarTypeInstanceContext : TypeInstanceContext {
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode TYPE_NAME() { return GetToken(TASONParser.TYPE_NAME, 0); }
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode STRING() { return GetToken(TASONParser.STRING, 0); }
		public ScalarTypeInstanceContext(TypeInstanceContext context) { CopyFrom(context); }
		[System.Diagnostics.DebuggerNonUserCode]
		public override void EnterRule(IParseTreeListener listener) {
			ITASONListener typedListener = listener as ITASONListener;
			if (typedListener != null) typedListener.EnterScalarTypeInstance(this);
		}
		[System.Diagnostics.DebuggerNonUserCode]
		public override void ExitRule(IParseTreeListener listener) {
			ITASONListener typedListener = listener as ITASONListener;
			if (typedListener != null) typedListener.ExitScalarTypeInstance(this);
		}
	}
	public partial class ObjectTypeInstanceContext : TypeInstanceContext {
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode TYPE_NAME() { return GetToken(TASONParser.TYPE_NAME, 0); }
		[System.Diagnostics.DebuggerNonUserCode] public ObjectContext @object() {
			return GetRuleContext<ObjectContext>(0);
		}
		public ObjectTypeInstanceContext(TypeInstanceContext context) { CopyFrom(context); }
		[System.Diagnostics.DebuggerNonUserCode]
		public override void EnterRule(IParseTreeListener listener) {
			ITASONListener typedListener = listener as ITASONListener;
			if (typedListener != null) typedListener.EnterObjectTypeInstance(this);
		}
		[System.Diagnostics.DebuggerNonUserCode]
		public override void ExitRule(IParseTreeListener listener) {
			ITASONListener typedListener = listener as ITASONListener;
			if (typedListener != null) typedListener.ExitObjectTypeInstance(this);
		}
	}

	[RuleVersion(0)]
	public TypeInstanceContext typeInstance() {
		TypeInstanceContext _localctx = new TypeInstanceContext(Context, State);
		EnterRule(_localctx, 6, RULE_typeInstance);
		try {
			State = 41;
			ErrorHandler.Sync(this);
			switch ( Interpreter.AdaptivePredict(TokenStream,1,Context) ) {
			case 1:
				_localctx = new ScalarTypeInstanceContext(_localctx);
				EnterOuterAlt(_localctx, 1);
				{
				State = 32;
				Match(TYPE_NAME);
				State = 33;
				Match(T__0);
				State = 34;
				Match(STRING);
				State = 35;
				Match(T__1);
				}
				break;
			case 2:
				_localctx = new ObjectTypeInstanceContext(_localctx);
				EnterOuterAlt(_localctx, 2);
				{
				State = 36;
				Match(TYPE_NAME);
				State = 37;
				Match(T__0);
				State = 38;
				@object();
				State = 39;
				Match(T__1);
				}
				break;
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	public partial class ObjectContext : ParserRuleContext {
		[System.Diagnostics.DebuggerNonUserCode] public PairContext[] pair() {
			return GetRuleContexts<PairContext>();
		}
		[System.Diagnostics.DebuggerNonUserCode] public PairContext pair(int i) {
			return GetRuleContext<PairContext>(i);
		}
		public ObjectContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_object; } }
		[System.Diagnostics.DebuggerNonUserCode]
		public override void EnterRule(IParseTreeListener listener) {
			ITASONListener typedListener = listener as ITASONListener;
			if (typedListener != null) typedListener.EnterObject(this);
		}
		[System.Diagnostics.DebuggerNonUserCode]
		public override void ExitRule(IParseTreeListener listener) {
			ITASONListener typedListener = listener as ITASONListener;
			if (typedListener != null) typedListener.ExitObject(this);
		}
	}

	[RuleVersion(0)]
	public ObjectContext @object() {
		ObjectContext _localctx = new ObjectContext(Context, State);
		EnterRule(_localctx, 8, RULE_object);
		int _la;
		try {
			int _alt;
			EnterOuterAlt(_localctx, 1);
			{
			State = 43;
			Match(T__2);
			State = 55;
			ErrorHandler.Sync(this);
			_la = TokenStream.LA(1);
			if (_la==IDENTIFIER || _la==STRING) {
				{
				State = 44;
				pair();
				State = 49;
				ErrorHandler.Sync(this);
				_alt = Interpreter.AdaptivePredict(TokenStream,2,Context);
				while ( _alt!=2 && _alt!=global::Antlr4.Runtime.Atn.ATN.INVALID_ALT_NUMBER ) {
					if ( _alt==1 ) {
						{
						{
						State = 45;
						Match(T__3);
						State = 46;
						pair();
						}
						} 
					}
					State = 51;
					ErrorHandler.Sync(this);
					_alt = Interpreter.AdaptivePredict(TokenStream,2,Context);
				}
				State = 53;
				ErrorHandler.Sync(this);
				_la = TokenStream.LA(1);
				if (_la==T__3) {
					{
					State = 52;
					Match(T__3);
					}
				}

				}
			}

			State = 57;
			Match(T__4);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	public partial class PairContext : ParserRuleContext {
		[System.Diagnostics.DebuggerNonUserCode] public KeyContext key() {
			return GetRuleContext<KeyContext>(0);
		}
		[System.Diagnostics.DebuggerNonUserCode] public ValueContext value() {
			return GetRuleContext<ValueContext>(0);
		}
		public PairContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_pair; } }
		[System.Diagnostics.DebuggerNonUserCode]
		public override void EnterRule(IParseTreeListener listener) {
			ITASONListener typedListener = listener as ITASONListener;
			if (typedListener != null) typedListener.EnterPair(this);
		}
		[System.Diagnostics.DebuggerNonUserCode]
		public override void ExitRule(IParseTreeListener listener) {
			ITASONListener typedListener = listener as ITASONListener;
			if (typedListener != null) typedListener.ExitPair(this);
		}
	}

	[RuleVersion(0)]
	public PairContext pair() {
		PairContext _localctx = new PairContext(Context, State);
		EnterRule(_localctx, 10, RULE_pair);
		try {
			EnterOuterAlt(_localctx, 1);
			{
			State = 59;
			key();
			State = 60;
			Match(T__5);
			State = 61;
			value();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	public partial class KeyContext : ParserRuleContext {
		public KeyContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_key; } }
	 
		public KeyContext() { }
		public virtual void CopyFrom(KeyContext context) {
			base.CopyFrom(context);
		}
	}
	public partial class IdentifierContext : KeyContext {
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode IDENTIFIER() { return GetToken(TASONParser.IDENTIFIER, 0); }
		public IdentifierContext(KeyContext context) { CopyFrom(context); }
		[System.Diagnostics.DebuggerNonUserCode]
		public override void EnterRule(IParseTreeListener listener) {
			ITASONListener typedListener = listener as ITASONListener;
			if (typedListener != null) typedListener.EnterIdentifier(this);
		}
		[System.Diagnostics.DebuggerNonUserCode]
		public override void ExitRule(IParseTreeListener listener) {
			ITASONListener typedListener = listener as ITASONListener;
			if (typedListener != null) typedListener.ExitIdentifier(this);
		}
	}
	public partial class StringKeyContext : KeyContext {
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode STRING() { return GetToken(TASONParser.STRING, 0); }
		public StringKeyContext(KeyContext context) { CopyFrom(context); }
		[System.Diagnostics.DebuggerNonUserCode]
		public override void EnterRule(IParseTreeListener listener) {
			ITASONListener typedListener = listener as ITASONListener;
			if (typedListener != null) typedListener.EnterStringKey(this);
		}
		[System.Diagnostics.DebuggerNonUserCode]
		public override void ExitRule(IParseTreeListener listener) {
			ITASONListener typedListener = listener as ITASONListener;
			if (typedListener != null) typedListener.ExitStringKey(this);
		}
	}

	[RuleVersion(0)]
	public KeyContext key() {
		KeyContext _localctx = new KeyContext(Context, State);
		EnterRule(_localctx, 12, RULE_key);
		try {
			State = 65;
			ErrorHandler.Sync(this);
			switch (TokenStream.LA(1)) {
			case STRING:
				_localctx = new StringKeyContext(_localctx);
				EnterOuterAlt(_localctx, 1);
				{
				State = 63;
				Match(STRING);
				}
				break;
			case IDENTIFIER:
				_localctx = new IdentifierContext(_localctx);
				EnterOuterAlt(_localctx, 2);
				{
				State = 64;
				Match(IDENTIFIER);
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	public partial class ArrayContext : ParserRuleContext {
		[System.Diagnostics.DebuggerNonUserCode] public ValueContext[] value() {
			return GetRuleContexts<ValueContext>();
		}
		[System.Diagnostics.DebuggerNonUserCode] public ValueContext value(int i) {
			return GetRuleContext<ValueContext>(i);
		}
		public ArrayContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_array; } }
		[System.Diagnostics.DebuggerNonUserCode]
		public override void EnterRule(IParseTreeListener listener) {
			ITASONListener typedListener = listener as ITASONListener;
			if (typedListener != null) typedListener.EnterArray(this);
		}
		[System.Diagnostics.DebuggerNonUserCode]
		public override void ExitRule(IParseTreeListener listener) {
			ITASONListener typedListener = listener as ITASONListener;
			if (typedListener != null) typedListener.ExitArray(this);
		}
	}

	[RuleVersion(0)]
	public ArrayContext array() {
		ArrayContext _localctx = new ArrayContext(Context, State);
		EnterRule(_localctx, 14, RULE_array);
		int _la;
		try {
			int _alt;
			State = 83;
			ErrorHandler.Sync(this);
			switch ( Interpreter.AdaptivePredict(TokenStream,8,Context) ) {
			case 1:
				EnterOuterAlt(_localctx, 1);
				{
				State = 67;
				Match(T__6);
				State = 68;
				value();
				State = 73;
				ErrorHandler.Sync(this);
				_alt = Interpreter.AdaptivePredict(TokenStream,6,Context);
				while ( _alt!=2 && _alt!=global::Antlr4.Runtime.Atn.ATN.INVALID_ALT_NUMBER ) {
					if ( _alt==1 ) {
						{
						{
						State = 69;
						Match(T__3);
						State = 70;
						value();
						}
						} 
					}
					State = 75;
					ErrorHandler.Sync(this);
					_alt = Interpreter.AdaptivePredict(TokenStream,6,Context);
				}
				State = 77;
				ErrorHandler.Sync(this);
				_la = TokenStream.LA(1);
				if (_la==T__3) {
					{
					State = 76;
					Match(T__3);
					}
				}

				State = 79;
				Match(T__7);
				}
				break;
			case 2:
				EnterOuterAlt(_localctx, 2);
				{
				State = 81;
				Match(T__6);
				State = 82;
				Match(T__7);
				}
				break;
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	public partial class NumberContext : ParserRuleContext {
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode NUMBER() { return GetToken(TASONParser.NUMBER, 0); }
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode SYMBOL() { return GetToken(TASONParser.SYMBOL, 0); }
		public NumberContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_number; } }
		[System.Diagnostics.DebuggerNonUserCode]
		public override void EnterRule(IParseTreeListener listener) {
			ITASONListener typedListener = listener as ITASONListener;
			if (typedListener != null) typedListener.EnterNumber(this);
		}
		[System.Diagnostics.DebuggerNonUserCode]
		public override void ExitRule(IParseTreeListener listener) {
			ITASONListener typedListener = listener as ITASONListener;
			if (typedListener != null) typedListener.ExitNumber(this);
		}
	}

	[RuleVersion(0)]
	public NumberContext number() {
		NumberContext _localctx = new NumberContext(Context, State);
		EnterRule(_localctx, 16, RULE_number);
		int _la;
		try {
			State = 97;
			ErrorHandler.Sync(this);
			switch ( Interpreter.AdaptivePredict(TokenStream,12,Context) ) {
			case 1:
				EnterOuterAlt(_localctx, 1);
				{
				State = 86;
				ErrorHandler.Sync(this);
				_la = TokenStream.LA(1);
				if (_la==SYMBOL) {
					{
					State = 85;
					Match(SYMBOL);
					}
				}

				State = 88;
				Match(NUMBER);
				}
				break;
			case 2:
				EnterOuterAlt(_localctx, 2);
				{
				State = 90;
				ErrorHandler.Sync(this);
				_la = TokenStream.LA(1);
				if (_la==SYMBOL) {
					{
					State = 89;
					Match(SYMBOL);
					}
				}

				State = 92;
				Match(T__8);
				}
				break;
			case 3:
				EnterOuterAlt(_localctx, 3);
				{
				State = 94;
				ErrorHandler.Sync(this);
				_la = TokenStream.LA(1);
				if (_la==SYMBOL) {
					{
					State = 93;
					Match(SYMBOL);
					}
				}

				State = 96;
				Match(T__9);
				}
				break;
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	private static int[] _serializedATN = {
		4,1,22,100,2,0,7,0,2,1,7,1,2,2,7,2,2,3,7,3,2,4,7,4,2,5,7,5,2,6,7,6,2,7,
		7,7,2,8,7,8,1,0,1,0,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,3,1,29,8,1,1,2,1,2,
		1,3,1,3,1,3,1,3,1,3,1,3,1,3,1,3,1,3,3,3,42,8,3,1,4,1,4,1,4,1,4,5,4,48,
		8,4,10,4,12,4,51,9,4,1,4,3,4,54,8,4,3,4,56,8,4,1,4,1,4,1,5,1,5,1,5,1,5,
		1,6,1,6,3,6,66,8,6,1,7,1,7,1,7,1,7,5,7,72,8,7,10,7,12,7,75,9,7,1,7,3,7,
		78,8,7,1,7,1,7,1,7,1,7,3,7,84,8,7,1,8,3,8,87,8,8,1,8,1,8,3,8,91,8,8,1,
		8,1,8,3,8,95,8,8,1,8,3,8,98,8,8,1,8,0,0,9,0,2,4,6,8,10,12,14,16,0,1,1,
		0,11,12,109,0,18,1,0,0,0,2,28,1,0,0,0,4,30,1,0,0,0,6,41,1,0,0,0,8,43,1,
		0,0,0,10,59,1,0,0,0,12,65,1,0,0,0,14,83,1,0,0,0,16,97,1,0,0,0,18,19,3,
		2,1,0,19,20,5,0,0,1,20,1,1,0,0,0,21,29,3,8,4,0,22,29,3,14,7,0,23,29,5,
		19,0,0,24,29,3,16,8,0,25,29,3,4,2,0,26,29,5,13,0,0,27,29,3,6,3,0,28,21,
		1,0,0,0,28,22,1,0,0,0,28,23,1,0,0,0,28,24,1,0,0,0,28,25,1,0,0,0,28,26,
		1,0,0,0,28,27,1,0,0,0,29,3,1,0,0,0,30,31,7,0,0,0,31,5,1,0,0,0,32,33,5,
		14,0,0,33,34,5,1,0,0,34,35,5,19,0,0,35,42,5,2,0,0,36,37,5,14,0,0,37,38,
		5,1,0,0,38,39,3,8,4,0,39,40,5,2,0,0,40,42,1,0,0,0,41,32,1,0,0,0,41,36,
		1,0,0,0,42,7,1,0,0,0,43,55,5,3,0,0,44,49,3,10,5,0,45,46,5,4,0,0,46,48,
		3,10,5,0,47,45,1,0,0,0,48,51,1,0,0,0,49,47,1,0,0,0,49,50,1,0,0,0,50,53,
		1,0,0,0,51,49,1,0,0,0,52,54,5,4,0,0,53,52,1,0,0,0,53,54,1,0,0,0,54,56,
		1,0,0,0,55,44,1,0,0,0,55,56,1,0,0,0,56,57,1,0,0,0,57,58,5,5,0,0,58,9,1,
		0,0,0,59,60,3,12,6,0,60,61,5,6,0,0,61,62,3,2,1,0,62,11,1,0,0,0,63,66,5,
		19,0,0,64,66,5,15,0,0,65,63,1,0,0,0,65,64,1,0,0,0,66,13,1,0,0,0,67,68,
		5,7,0,0,68,73,3,2,1,0,69,70,5,4,0,0,70,72,3,2,1,0,71,69,1,0,0,0,72,75,
		1,0,0,0,73,71,1,0,0,0,73,74,1,0,0,0,74,77,1,0,0,0,75,73,1,0,0,0,76,78,
		5,4,0,0,77,76,1,0,0,0,77,78,1,0,0,0,78,79,1,0,0,0,79,80,5,8,0,0,80,84,
		1,0,0,0,81,82,5,7,0,0,82,84,5,8,0,0,83,67,1,0,0,0,83,81,1,0,0,0,84,15,
		1,0,0,0,85,87,5,20,0,0,86,85,1,0,0,0,86,87,1,0,0,0,87,88,1,0,0,0,88,98,
		5,21,0,0,89,91,5,20,0,0,90,89,1,0,0,0,90,91,1,0,0,0,91,92,1,0,0,0,92,98,
		5,9,0,0,93,95,5,20,0,0,94,93,1,0,0,0,94,95,1,0,0,0,95,96,1,0,0,0,96,98,
		5,10,0,0,97,86,1,0,0,0,97,90,1,0,0,0,97,94,1,0,0,0,98,17,1,0,0,0,13,28,
		41,49,53,55,65,73,77,83,86,90,94,97
	};

	public static readonly ATN _ATN =
		new ATNDeserializer().Deserialize(_serializedATN);


}
} // namespace TASON.Grammar
