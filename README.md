# WallPaper-WeekdayFarm
Wallpaper Engine용 트릭컬 평일농장 배경화면

## 🖥️ 프로젝트 소개
모바일 게임 '트릭컬'의 컨텐츠 평일농장을 배경화면으로 사용할 수 있도록 Unity 2D에서 구현한 프로젝트입니다.


![unnamed](https://github.com/user-attachments/assets/b1273554-6279-4eb7-a4ac-f612680a450b)

## ⏰ 개발 기간
* 24.09.02. ~ 24.10.02. (이후 사도 지속적으로 업데이트)

## ⚙️ 개발 환경
* Unity6 (6000.0.12f1)

## 🦾 주요 기능
### 사도 생성 및 이동
- 사도 이동은 FSM 기반으로 NavMeshPlus를 이용하여 구현
- 정지했을 때 다음 목표 지점을 설정
- 처음에 부여된 무작위 값만큼 이동 횟수를 달성하면 화면 밖으로 이동하여 사도 교체
- 사도를 교체할 때 오브젝트를 새로 생성하거나 삭제하지 않고 풀링 적용
### 설정
![화면 캡처 2024-11-09 200130](https://github.com/user-attachments/assets/d0a0cae2-3503-40e8-9de1-2af0875048fa)
- 그루터기를 클릭하면 설정 UI가 활성화
- HashSet에 이름을 넣고 빼며 등장할 사도 설정
### 시간 표시
![image](https://github.com/user-attachments/assets/c58edbcb-99b1-4703-b9ac-782ea8c3388a)
- 기기의 날짜와 시간을 가공하여 표시
