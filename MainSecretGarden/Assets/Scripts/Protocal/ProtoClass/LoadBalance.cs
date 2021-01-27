// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: LoadBalance.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace Game.Protocal {

  /// <summary>Holder for reflection information generated from LoadBalance.proto</summary>
  public static partial class LoadBalanceReflection {

    #region Descriptor
    /// <summary>File descriptor for LoadBalance.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static LoadBalanceReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "ChFMb2FkQmFsYW5jZS5wcm90bxINR2FtZS5Qcm90b2NhbBoORW51bVR5cGUu",
            "cHJvdG8iTQoKQ1NMaW5rSW5mbxIMCgRTb2xlGAEgASgJEjEKCFBsYXRmb3Jt",
            "GAIgASgOMh8uR2FtZS5Qcm90b2NhbC5SZWdpc3RlclBsYXRmb3JtInAKClND",
            "TGlua0luZm8SCgoCSXAYASABKAQSDAoEUG9ydBgCIAEoBRIrCgZTdGF0dXMY",
            "AyABKA4yGy5HYW1lLlByb3RvY2FsLlNlcnZlclN0YXR1cxILCgNVaWQYBCAB",
            "KAMSDgoGTm90aWNlGAUgASgJYgZwcm90bzM="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { global::Game.Protocal.EnumTypeReflection.Descriptor, },
          new pbr::GeneratedClrTypeInfo(null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::Game.Protocal.CSLinkInfo), global::Game.Protocal.CSLinkInfo.Parser, new[]{ "Sole", "Platform" }, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::Game.Protocal.SCLinkInfo), global::Game.Protocal.SCLinkInfo.Parser, new[]{ "Ip", "Port", "Status", "Uid", "Notice" }, null, null, null)
          }));
    }
    #endregion

  }
  #region Messages
  /// <summary>
  /// 请求获取
  /// </summary>
  public sealed partial class CSLinkInfo : pb::IMessage<CSLinkInfo> {
    private static readonly pb::MessageParser<CSLinkInfo> _parser = new pb::MessageParser<CSLinkInfo>(() => new CSLinkInfo());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<CSLinkInfo> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Game.Protocal.LoadBalanceReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public CSLinkInfo() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public CSLinkInfo(CSLinkInfo other) : this() {
      sole_ = other.sole_;
      platform_ = other.platform_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public CSLinkInfo Clone() {
      return new CSLinkInfo(this);
    }

    /// <summary>Field number for the "Sole" field.</summary>
    public const int SoleFieldNumber = 1;
    private string sole_ = "";
    /// <summary>
    /// 唯一标示
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string Sole {
      get { return sole_; }
      set {
        sole_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "Platform" field.</summary>
    public const int PlatformFieldNumber = 2;
    private global::Game.Protocal.RegisterPlatform platform_ = 0;
    /// <summary>
    /// 登录平台
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Game.Protocal.RegisterPlatform Platform {
      get { return platform_; }
      set {
        platform_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as CSLinkInfo);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(CSLinkInfo other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (Sole != other.Sole) return false;
      if (Platform != other.Platform) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (Sole.Length != 0) hash ^= Sole.GetHashCode();
      if (Platform != 0) hash ^= Platform.GetHashCode();
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
      if (Sole.Length != 0) {
        output.WriteRawTag(10);
        output.WriteString(Sole);
      }
      if (Platform != 0) {
        output.WriteRawTag(16);
        output.WriteEnum((int) Platform);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (Sole.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(Sole);
      }
      if (Platform != 0) {
        size += 1 + pb::CodedOutputStream.ComputeEnumSize((int) Platform);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(CSLinkInfo other) {
      if (other == null) {
        return;
      }
      if (other.Sole.Length != 0) {
        Sole = other.Sole;
      }
      if (other.Platform != 0) {
        Platform = other.Platform;
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
            Sole = input.ReadString();
            break;
          }
          case 16: {
            Platform = (global::Game.Protocal.RegisterPlatform) input.ReadEnum();
            break;
          }
        }
      }
    }

  }

  /// <summary>
  /// 返回获取
  /// </summary>
  public sealed partial class SCLinkInfo : pb::IMessage<SCLinkInfo> {
    private static readonly pb::MessageParser<SCLinkInfo> _parser = new pb::MessageParser<SCLinkInfo>(() => new SCLinkInfo());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<SCLinkInfo> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Game.Protocal.LoadBalanceReflection.Descriptor.MessageTypes[1]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public SCLinkInfo() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public SCLinkInfo(SCLinkInfo other) : this() {
      ip_ = other.ip_;
      port_ = other.port_;
      status_ = other.status_;
      uid_ = other.uid_;
      notice_ = other.notice_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public SCLinkInfo Clone() {
      return new SCLinkInfo(this);
    }

    /// <summary>Field number for the "Ip" field.</summary>
    public const int IpFieldNumber = 1;
    private ulong ip_;
    /// <summary>
    /// ip地址
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public ulong Ip {
      get { return ip_; }
      set {
        ip_ = value;
      }
    }

    /// <summary>Field number for the "Port" field.</summary>
    public const int PortFieldNumber = 2;
    private int port_;
    /// <summary>
    /// 端口
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int Port {
      get { return port_; }
      set {
        port_ = value;
      }
    }

    /// <summary>Field number for the "Status" field.</summary>
    public const int StatusFieldNumber = 3;
    private global::Game.Protocal.ServerStatus status_ = 0;
    /// <summary>
    /// 服务器状态
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Game.Protocal.ServerStatus Status {
      get { return status_; }
      set {
        status_ = value;
      }
    }

    /// <summary>Field number for the "Uid" field.</summary>
    public const int UidFieldNumber = 4;
    private long uid_;
    /// <summary>
    /// 玩家uid
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public long Uid {
      get { return uid_; }
      set {
        uid_ = value;
      }
    }

    /// <summary>Field number for the "Notice" field.</summary>
    public const int NoticeFieldNumber = 5;
    private string notice_ = "";
    /// <summary>
    /// 公告
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string Notice {
      get { return notice_; }
      set {
        notice_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as SCLinkInfo);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(SCLinkInfo other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (Ip != other.Ip) return false;
      if (Port != other.Port) return false;
      if (Status != other.Status) return false;
      if (Uid != other.Uid) return false;
      if (Notice != other.Notice) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (Ip != 0UL) hash ^= Ip.GetHashCode();
      if (Port != 0) hash ^= Port.GetHashCode();
      if (Status != 0) hash ^= Status.GetHashCode();
      if (Uid != 0L) hash ^= Uid.GetHashCode();
      if (Notice.Length != 0) hash ^= Notice.GetHashCode();
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
      if (Ip != 0UL) {
        output.WriteRawTag(8);
        output.WriteUInt64(Ip);
      }
      if (Port != 0) {
        output.WriteRawTag(16);
        output.WriteInt32(Port);
      }
      if (Status != 0) {
        output.WriteRawTag(24);
        output.WriteEnum((int) Status);
      }
      if (Uid != 0L) {
        output.WriteRawTag(32);
        output.WriteInt64(Uid);
      }
      if (Notice.Length != 0) {
        output.WriteRawTag(42);
        output.WriteString(Notice);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (Ip != 0UL) {
        size += 1 + pb::CodedOutputStream.ComputeUInt64Size(Ip);
      }
      if (Port != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(Port);
      }
      if (Status != 0) {
        size += 1 + pb::CodedOutputStream.ComputeEnumSize((int) Status);
      }
      if (Uid != 0L) {
        size += 1 + pb::CodedOutputStream.ComputeInt64Size(Uid);
      }
      if (Notice.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(Notice);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(SCLinkInfo other) {
      if (other == null) {
        return;
      }
      if (other.Ip != 0UL) {
        Ip = other.Ip;
      }
      if (other.Port != 0) {
        Port = other.Port;
      }
      if (other.Status != 0) {
        Status = other.Status;
      }
      if (other.Uid != 0L) {
        Uid = other.Uid;
      }
      if (other.Notice.Length != 0) {
        Notice = other.Notice;
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
            Ip = input.ReadUInt64();
            break;
          }
          case 16: {
            Port = input.ReadInt32();
            break;
          }
          case 24: {
            Status = (global::Game.Protocal.ServerStatus) input.ReadEnum();
            break;
          }
          case 32: {
            Uid = input.ReadInt64();
            break;
          }
          case 42: {
            Notice = input.ReadString();
            break;
          }
        }
      }
    }

  }

  #endregion

}

#endregion Designer generated code