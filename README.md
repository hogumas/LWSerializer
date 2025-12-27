\# LWSerializer



\[!\[License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

!\[.NET](https://img.shields.io/badge/.NET-8.0%2B-blueviolet)

!\[Platform](https://img.shields.io/badge/Platform-Cross--platform-lightgrey)



\*\*LWSerializer\*\*는 .NET 환경에서 극한의 성능을 추구하는 개발자를 위한 \*\*경량 고성능 바이너리 직렬화 라이브러리\*\*입니다. 메모리 직접 복사 방식을 통해 최소한의 오버헤드로 데이터를 처리하며, 게임 개발 및 고성능 서버 환경에 최적화되어 있습니다.



---



\## ✨ Key Features



\* \*\*Extreme Performance\*\*: `MemoryPack`을 뛰어넘는 압도적인 직렬화/역직렬화 속도를 제공합니다.

\* \*\*Simple \& Intuitive\*\*: 복잡한 설정 없이 `LwBinaryWriter/Reader`를 통해 즉시 사용 가능합니다.

\* \*\*Manual Ordering\*\*: 데이터의 직렬화 순서를 사용자가 직접 제어할 수 있어 유연한 프로토콜 설계가 가능합니다.

\* \*\*Version Tolerance (Padding)\*\*: 구조체 변경에 대비한 Padding 기능을 지원하여, 데이터 구조 확장 시에도 하위 호환성을 유지할 수 있습니다.

\* \*\*Memory Efficiency\*\*:

&nbsp;   \* 모든 `unmanaged` 구조체 직렬화 지원

&nbsp;   \* Memory Aliasing 대응을 통한 안전하고 효율적인 메모리 접근

&nbsp;   \* 최대 \*\*2GB\*\* 크기의 대용량 데이터 직렬화 가능

\* \*\*Collection Support\*\*: `List<T>`, `Dictionary<TKey, TValue>` 등 주요 제네릭 컬렉션을 기본 지원합니다.



---



\## 🚀 Performance Comparison



LWSerializer는 직접적인 메모리 복사 메커니즘을 사용하여 기존의 대중적인 라이브러리들보다 빠른 속도를 자랑합니다.



| Library | Performance Tier | Methodology |

| :--- | :---: | :--- |

| \*\*LWSerializer\*\* | 🚀 \*\*God Tier\*\* | \*\*Direct Memory Copy / Low Overhead\*\* |

| MemoryPack | High | Zero-order Reflection / Code Gen |

| Protobuf-net | Medium | Contract Based |

| Json.NET | Low | Text Based |



---



\## 🛠 Usage



\### Basic Struct Serialization



`unmanaged` 구조체를 바이너리로 변환하고 다시 복구하는 예제입니다.



```csharp

public struct ExampleStruct

{

&nbsp;   private int \_firstInt;

&nbsp;   private float \_firstFloat;

&nbsp;   private bool \_bool;

&nbsp;   private decimal \_decimal;

}



public static class Example

{

&nbsp;   /// <summary> ExampleStruct(unmanaged)를 직렬화합니다. </summary>

&nbsp;   public static byte\[] Serialize(ExampleStruct exampleStruct)

&nbsp;   {

&nbsp;       using (var writer = new LwBinaryWriter())

&nbsp;       {

&nbsp;           writer.Write(exampleStruct);

&nbsp;           return writer.ToArray(); // 또는 고성능 처리를 위해 writer.ToPtr() 사용 가능

&nbsp;       }

&nbsp;   }



&nbsp;   /// <summary> 바이너리 데이터를 다시 구조체로 역직렬화합니다. </summary>

&nbsp;   public static ExampleStruct Deserialize(byte\[] bytes)

&nbsp;   {

&nbsp;       using (var reader = new LwBinaryReader(bytes))

&nbsp;       {

&nbsp;           reader.Read(out ExampleStruct result);

&nbsp;           return result;

&nbsp;       }

&nbsp;   }

}

