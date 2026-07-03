# Fierce Combat ONLINE

Unity 2D 기반 온라인 실시간 대전 액션 게임입니다. Photon Unity Networking(PUN2)을 이용해 멀티플레이 전투, 로비/채팅, 스테이지 동기화를 구현했습니다.

## 개발 환경

- **Engine**: Unity 2022.3.7f1 (URP)
- **Networking**: Photon Unity Networking 2 (PUN2), Photon Chat
- **UI**: TextMeshPro
- **Rendering**: Universal Render Pipeline, ProPixelizer(픽셀 아트 아웃라인/쉐이딩)

## 주요 기능

- **전투 시스템**: 검/총/쿠나이 등 무기별 스킬 슬롯 관리, MP 게이지 기반 필살기(궁극기) 발동 (`SkillManager`, `Entity`)
- **네트워크 동기화**: 캐릭터 위치·애니메이션·스프라이트 색상 등 실시간 동기화 (`NetworkSpriteColorChanger`, `AnimationManager`)
- **로비/채팅**: 대기방, 비밀방(코드 입력) 생성 및 룸 리스트 동기화, 인게임 채팅 (`ChatService`, `WaittingRoomCtrl`)
- **스테이지/맵 오브젝트**: 맵별 기믹 오브젝트(성, 물고기, 폭탄통, 몬스터 등) 및 카메라 연출 (`StageCtrl`, `DynamicCamera`)
- **이펙트/사운드**: 스킬 이펙트 생성 및 관리, 사운드 매니저 (`EffectCreator`, `SoundManager`)

## 프로젝트 구조

```
Assets/
├── Hyun/          # 전투(엔티티, 스킬, 애니메이션, 이펙트), 채팅 서비스
├── Mingyu/         # 대기방 UI, 맵별 오브젝트(도시/성/DK 등) 연출
├── SeukHan/        # 스테이지 컨트롤, 맵 오브젝트, UI
├── Photon/         # PUN2 / Photon Chat 라이브러리
├── ProPixelizer/    # 픽셀 아트 스타일 렌더링
├── 2D Platfromer/   # 2D 플랫포머 베이스 캐릭터 컨트롤
└── Hovl Studio, 25 sprite effects/  # 이펙트 에셋
```

## 개발 히스토리 (요약)

- 로비/채팅 동기화 및 비밀방(코드 입력) 기능 구현
- 캐릭터 전투 네트워크 동기화 및 애니메이션 동기화 완료
- 몬스터 네트워크 동기화 구현
- 스킬/필살기 시스템 구현 및 버그 수정 (내리찍기, 필살기 등)
- 각 맵(스테이지)별 기믹 오브젝트 구현
