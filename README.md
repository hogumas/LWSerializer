.net 환경에서 값을 복사하여 바이너리로 변경하는 직렬화 라이브러리입니다.


- 특징
  - 사용법이 간단합니다
  - 직렬화 순서를 직접 지정 할 수 있습니다
  - Padding 기능을 활용하여 추후 구조가 바뀜에의해 사이즈가 바뀌는것을 대응 할수 있습니다

- 지원기능
  - 모든 unmanaged 구조체 직렬화 지원
  - 관리되는 형식을 직렬화 하기위한 방법 지원
  - Memory Aliasing 대응
  - 최대 2GB까지 직렬화가 가능합니다

- 성능
  - Json < Protobuf < MemoryPack < LWBinarySerializer
 
  
