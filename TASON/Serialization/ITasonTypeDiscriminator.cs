using System;
using System.Collections.Generic;
using System.Text;

namespace TASON.Serialization;

/// <summary>表示一个TASON类型判别器，可以实现多态序列化</summary>
public interface ITasonTypeDiscriminator
{
    /// <summary>获取当前对象序列化所使用的TASON类型名称</summary> 
    string GetTypeName();
}
