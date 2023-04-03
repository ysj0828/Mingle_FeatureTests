# Mingle_FeatureTests
밍글 프로젝트 베타 개발전 어떤 기능 넣을지 테스트한 프로젝트

스크립트 정리 X



# 본인이 테스트한 기능 목록 (현재 repo에서 테스트 한 내용과 실제 프로젝트에서 변경된 시나리오 모두 포함)
### 테스트 끝난 후 실제 프로젝트에서 맡은 업무 : 캐릭터 관련 (이동, 감정표현 애니메이션, 오브젝트 상호작용 애니메이션, 포톤관련 기능 : 캐릭터 싱크, 유니티 Resources 폴더 밖에서 PhotonNetwork.Instantiate 할 수 있는 방법, etc)

1. 아바타 이동 : Nav Mesh Agent로 결정

2. 방 편집 : 물리 구현 할지 말지 - 물리 적용 안하기로 결정\
2-1 실시간 방 편집 테스트\
2-2. 방 편집 중 : 다른 유저에게 어떻게 표시할 지 - 편집 중인 오브젝트 위에 스패너 에셋 띄우고 편집자 화면에선 표시 안하기로 결정\
2-3. 비동기화 방 편집으로 시나리오 변경 : 편집 끝나면 Scene 다시 로드해서 편집 완료된 방으로 다시 로드

3. 카메라 : 유니티 빌트인 카메라대신 Cinemachine 사용하기로 결정\
3-1. 1인칭 모드 : 자이로스코프 기능 사용할 지 - 자이로스코프의 Recalibration까지 구현은 됐으나 Recalibration이 필요한 시점의 시나리오가 애매해서 사용안하기로 결정

4. 아트팀에서 캐릭터 & 애니메이션 제작관련 테스트 요청 : 같은 애니메이션으로 캐릭터의 bone 구조 바꿨을 때 or 같은 캐릭터로 애니메이션의 구조 바꿨을 때 정상 작동하는지 테스트 (어떤 구조를 바꿨는지 자세히는 모름)

5. Photon 사용해서 캐릭터 이동, 회전 값, 애니메이션 등 싱크 제대로 맞는지 : Photon Transform/Animator View 사용했을 때 싱크가 잘 맞지 않아 우선 RPC 사용. 예 : 캐릭터가 (0,0,0)좌표로 이동하면 직접적으로 Transform 값을 직접 동기화하는게 아닌 RPC로 각 기기에 해당 캐릭터 이동 명령을 보내 NavMeshAgent.SetDestitnaion() 함수로 싱크 맞춤

6. 캐릭터가 방 오브젝트 뒤에 있어서 보이지 않을 경우 : 오브젝트 투명화 & 캐릭터 아웃라인/실루엣 표시 테스트 - 둘 다 사용하지 않고 각 캐릭터 밑에 닉네임을 표시하기로 결정
