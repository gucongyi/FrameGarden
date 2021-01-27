// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: RichMan.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace Game.Protocal {

  /// <summary>Holder for reflection information generated from RichMan.proto</summary>
  public static partial class RichManReflection {

    #region Descriptor
    /// <summary>File descriptor for RichMan.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static RichManReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "Cg1SaWNoTWFuLnByb3RvEg1HYW1lLlByb3RvY2FsGg5FbnVtVHlwZS5wcm90",
            "bxoTU3RhZ2VQcm9wZXJ0eS5wcm90byIbCgpDU0VudGVyTWFwEg0KBU1hcElk",
            "GAEgASgFIi8KBkNTRGljZRIlCgRkaWNlGAEgASgOMhcuR2FtZS5Qcm90b2Nh",
            "bC5EaWNlVHlwZSJKCgxTQ0RpY2VTdHJ1Y3QSDwoHRGljZU51bRgBIAEoBRIQ",
            "CghMb2NhdGlvbhgCIAMoBRIXCg9QcmVzZW50TG9jYXRpb24YAyABKAUiOQoM",
            "U0NEaWNlUmVzdWx0EikKBEluZm8YASADKAsyGy5HYW1lLlByb3RvY2FsLlND",
            "RGljZVN0cnVjdCJBCgdDU0x1Y2t5EiYKBFR5cGUYASABKA4yGC5HYW1lLlBy",
            "b3RvY2FsLkx1Y2t5VHllcBIOCgZHb29kSWQYAiADKAUiOAoHU0NMdWNreRIt",
            "CgRJbmZvGAEgASgLMh8uR2FtZS5Qcm90b2NhbC5TQ0J1eUdvb2RzU3RydWN0",
            "YgZwcm90bzM="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { global::Game.Protocal.EnumTypeReflection.Descriptor, global::Game.Protocal.StagePropertyReflection.Descriptor, },
          new pbr::GeneratedClrTypeInfo(null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::Game.Protocal.CSEnterMap), global::Game.Protocal.CSEnterMap.Parser, new[]{ "MapId" }, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::Game.Protocal.CSDice), global::Game.Protocal.CSDice.Parser, new[]{ "Dice" }, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::Game.Protocal.SCDiceStruct), global::Game.Protocal.SCDiceStruct.Parser, new[]{ "DiceNum", "Location", "PresentLocation" }, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::Game.Protocal.SCDiceResult), global::Game.Protocal.SCDiceResult.Parser, new[]{ "Info" }, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::Game.Protocal.CSLucky), global::Game.Protocal.CSLucky.Parser, new[]{ "Type", "GoodId" }, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::Game.Protocal.SCLucky), global::Game.Protocal.SCLucky.Parser, new[]{ "Info" }, null, null, null)
          }));
    }
    #endregion

  }
  #region Messages
  /// <summary>
  /// 请求进入大富翁地图
  /// </summary>
  public sealed partial class CSEnterMap : pb::IMessage<CSEnterMap> {
    private static readonly pb::MessageParser<CSEnterMap> _parser = new pb::MessageParser<CSEnterMap>(() => new CSEnterMap());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<CSEnterMap> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Game.Protocal.RichManReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public CSEnterMap() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public CSEnterMap(CSEnterMap other) : this() {
      mapId_ = other.mapId_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public CSEnterMap Clone() {
      return new CSEnterMap(this);
    }

    /// <summary>Field number for the "MapId" field.</summary>
    public const int MapIdFieldNumber = 1;
    private int mapId_;
    /// <summary>
    /// 地图id
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int MapId {
      get { return mapId_; }
      set {
        mapId_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as CSEnterMap);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(CSEnterMap other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (MapId != other.MapId) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (MapId != 0) hash ^= MapId.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (MapId != 0) {
        output.WriteRawTag(8);
        output.WriteInt32(MapId);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (MapId != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(MapId);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(CSEnterMap other) {
      if (other == null) {
        return;
      }
      if (other.MapId != 0) {
        MapId = other.MapId;
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 8: {
            MapId = input.ReadInt32();
            break;
          }
        }
      }
    }

  }

  /// <summary>
  /// 请求骰子类型
  /// </summary>
  public sealed partial class CSDice : pb::IMessage<CSDice> {
    private static readonly pb::MessageParser<CSDice> _parser = new pb::MessageParser<CSDice>(() => new CSDice());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<CSDice> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Game.Protocal.RichManReflection.Descriptor.MessageTypes[1]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public CSDice() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public CSDice(CSDice other) : this() {
      dice_ = other.dice_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public CSDice Clone() {
      return new CSDice(this);
    }

    /// <summary>Field number for the "dice" field.</summary>
    public const int DiceFieldNumber = 1;
    private global::Game.Protocal.DiceType dice_ = 0;
    /// <summary>
    /// 筛子类型
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Game.Protocal.DiceType Dice {
      get { return dice_; }
      set {
        dice_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as CSDice);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(CSDice other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (Dice != other.Dice) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (Dice != 0) hash ^= Dice.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (Dice != 0) {
        output.WriteRawTag(8);
        output.WriteEnum((int) Dice);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (Dice != 0) {
        size += 1 + pb::CodedOutputStream.ComputeEnumSize((int) Dice);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(CSDice other) {
      if (other == null) {
        return;
      }
      if (other.Dice != 0) {
        Dice = other.Dice;
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 8: {
            Dice = (global::Game.Protocal.DiceType) input.ReadEnum();
            break;
          }
        }
      }
    }

  }

  /// <summary>
  /// 骰子信息结果
  /// </summary>
  public sealed partial class SCDiceStruct : pb::IMessage<SCDiceStruct> {
    private static readonly pb::MessageParser<SCDiceStruct> _parser = new pb::MessageParser<SCDiceStruct>(() => new SCDiceStruct());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<SCDiceStruct> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Game.Protocal.RichManReflection.Descriptor.MessageTypes[2]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public SCDiceStruct() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public SCDiceStruct(SCDiceStruct other) : this() {
      diceNum_ = other.diceNum_;
      location_ = other.location_.Clone();
      presentLocation_ = other.presentLocation_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public SCDiceStruct Clone() {
      return new SCDiceStruct(this);
    }

    /// <summary>Field number for the "DiceNum" field.</summary>
    public const int DiceNumFieldNumber = 1;
    private int diceNum_;
    /// <summary>
    /// 骰子数
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int DiceNum {
      get { return diceNum_; }
      set {
        diceNum_ = value;
      }
    }

    /// <summary>Field number for the "Location" field.</summary>
    public const int LocationFieldNumber = 2;
    private static readonly pb::FieldCodec<int> _repeated_location_codec
        = pb::FieldCodec.ForInt32(18);
    private readonly pbc::RepeatedField<int> location_ = new pbc::RepeatedField<int>();
    /// <summary>
    /// 事件位置
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::RepeatedField<int> Location {
      get { return location_; }
    }

    /// <summary>Field number for the "PresentLocation" field.</summary>
    public const int PresentLocationFieldNumber = 3;
    private int presentLocation_;
    /// <summary>
    /// 当前位置
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int PresentLocation {
      get { return presentLocation_; }
      set {
        presentLocation_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as SCDiceStruct);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(SCDiceStruct other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (DiceNum != other.DiceNum) return false;
      if(!location_.Equals(other.location_)) return false;
      if (PresentLocation != other.PresentLocation) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (DiceNum != 0) hash ^= DiceNum.GetHashCode();
      hash ^= location_.GetHashCode();
      if (PresentLocation != 0) hash ^= PresentLocation.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (DiceNum != 0) {
        output.WriteRawTag(8);
        output.WriteInt32(DiceNum);
      }
      location_.WriteTo(output, _repeated_location_codec);
      if (PresentLocation != 0) {
        output.WriteRawTag(24);
        output.WriteInt32(PresentLocation);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (DiceNum != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(DiceNum);
      }
      size += location_.CalculateSize(_repeated_location_codec);
      if (PresentLocation != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(PresentLocation);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(SCDiceStruct other) {
      if (other == null) {
        return;
      }
      if (other.DiceNum != 0) {
        DiceNum = other.DiceNum;
      }
      location_.Add(other.location_);
      if (other.PresentLocation != 0) {
        PresentLocation = other.PresentLocation;
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 8: {
            DiceNum = input.ReadInt32();
            break;
          }
          case 18:
          case 16: {
            location_.AddEntriesFrom(input, _repeated_location_codec);
            break;
          }
          case 24: {
            PresentLocation = input.ReadInt32();
            break;
          }
        }
      }
    }

  }

  /// <summary>
  /// 返回骰子结果
  /// </summary>
  public sealed partial class SCDiceResult : pb::IMessage<SCDiceResult> {
    private static readonly pb::MessageParser<SCDiceResult> _parser = new pb::MessageParser<SCDiceResult>(() => new SCDiceResult());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<SCDiceResult> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Game.Protocal.RichManReflection.Descriptor.MessageTypes[3]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public SCDiceResult() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public SCDiceResult(SCDiceResult other) : this() {
      info_ = other.info_.Clone();
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public SCDiceResult Clone() {
      return new SCDiceResult(this);
    }

    /// <summary>Field number for the "Info" field.</summary>
    public const int InfoFieldNumber = 1;
    private static readonly pb::FieldCodec<global::Game.Protocal.SCDiceStruct> _repeated_info_codec
        = pb::FieldCodec.ForMessage(10, global::Game.Protocal.SCDiceStruct.Parser);
    private readonly pbc::RepeatedField<global::Game.Protocal.SCDiceStruct> info_ = new pbc::RepeatedField<global::Game.Protocal.SCDiceStruct>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::RepeatedField<global::Game.Protocal.SCDiceStruct> Info {
      get { return info_; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as SCDiceResult);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(SCDiceResult other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if(!info_.Equals(other.info_)) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      hash ^= info_.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      info_.WriteTo(output, _repeated_info_codec);
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      size += info_.CalculateSize(_repeated_info_codec);
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(SCDiceResult other) {
      if (other == null) {
        return;
      }
      info_.Add(other.info_);
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 10: {
            info_.AddEntriesFrom(input, _repeated_info_codec);
            break;
          }
        }
      }
    }

  }

  /// <summary>
  /// 请求抽道具或种子功能
  /// </summary>
  public sealed partial class CSLucky : pb::IMessage<CSLucky> {
    private static readonly pb::MessageParser<CSLucky> _parser = new pb::MessageParser<CSLucky>(() => new CSLucky());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<CSLucky> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Game.Protocal.RichManReflection.Descriptor.MessageTypes[4]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public CSLucky() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public CSLucky(CSLucky other) : this() {
      type_ = other.type_;
      goodId_ = other.goodId_.Clone();
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public CSLucky Clone() {
      return new CSLucky(this);
    }

    /// <summary>Field number for the "Type" field.</summary>
    public const int TypeFieldNumber = 1;
    private global::Game.Protocal.LuckyTyep type_ = 0;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Game.Protocal.LuckyTyep Type {
      get { return type_; }
      set {
        type_ = value;
      }
    }

    /// <summary>Field number for the "GoodId" field.</summary>
    public const int GoodIdFieldNumber = 2;
    private static readonly pb::FieldCodec<int> _repeated_goodId_codec
        = pb::FieldCodec.ForInt32(18);
    private readonly pbc::RepeatedField<int> goodId_ = new pbc::RepeatedField<int>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::RepeatedField<int> GoodId {
      get { return goodId_; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as CSLucky);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(CSLucky other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (Type != other.Type) return false;
      if(!goodId_.Equals(other.goodId_)) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (Type != 0) hash ^= Type.GetHashCode();
      hash ^= goodId_.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (Type != 0) {
        output.WriteRawTag(8);
        output.WriteEnum((int) Type);
      }
      goodId_.WriteTo(output, _repeated_goodId_codec);
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (Type != 0) {
        size += 1 + pb::CodedOutputStream.ComputeEnumSize((int) Type);
      }
      size += goodId_.CalculateSize(_repeated_goodId_codec);
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(CSLucky other) {
      if (other == null) {
        return;
      }
      if (other.Type != 0) {
        Type = other.Type;
      }
      goodId_.Add(other.goodId_);
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 8: {
            Type = (global::Game.Protocal.LuckyTyep) input.ReadEnum();
            break;
          }
          case 18:
          case 16: {
            goodId_.AddEntriesFrom(input, _repeated_goodId_codec);
            break;
          }
        }
      }
    }

  }

  /// <summary>
  /// 返回抽道具或种子信息
  /// </summary>
  public sealed partial class SCLucky : pb::IMessage<SCLucky> {
    private static readonly pb::MessageParser<SCLucky> _parser = new pb::MessageParser<SCLucky>(() => new SCLucky());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<SCLucky> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Game.Protocal.RichManReflection.Descriptor.MessageTypes[5]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public SCLucky() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public SCLucky(SCLucky other) : this() {
      info_ = other.info_ != null ? other.info_.Clone() : null;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public SCLucky Clone() {
      return new SCLucky(this);
    }

    /// <summary>Field number for the "Info" field.</summary>
    public const int InfoFieldNumber = 1;
    private global::Game.Protocal.SCBuyGoodsStruct info_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Game.Protocal.SCBuyGoodsStruct Info {
      get { return info_; }
      set {
        info_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as SCLucky);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(SCLucky other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (!object.Equals(Info, other.Info)) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (info_ != null) hash ^= Info.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (info_ != null) {
        output.WriteRawTag(10);
        output.WriteMessage(Info);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (info_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(Info);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(SCLucky other) {
      if (other == null) {
        return;
      }
      if (other.info_ != null) {
        if (info_ == null) {
          Info = new global::Game.Protocal.SCBuyGoodsStruct();
        }
        Info.MergeFrom(other.Info);
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 10: {
            if (info_ == null) {
              Info = new global::Game.Protocal.SCBuyGoodsStruct();
            }
            input.ReadMessage(Info);
            break;
          }
        }
      }
    }

  }

  #endregion

}

#endregion Designer generated code