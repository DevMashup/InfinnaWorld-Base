using TMPro;
using UnityEngine;

namespace DevMashup {
    namespace GamePlay {
        [CreateAssetMenu(fileName = "Extra", menuName = "InfinnaWorld/Extra")]
        public class Extra : ScriptableObject {
            public Equinox[] Equinoxes;
            private int dayLength { get { return Equinoxes[0].sunSet - Equinoxes[0].sunRise; } }
            private float sunDayRotationPerMinute { get { return 180f / dayLength; } }
            private float sunNightRotationPerMinute { get { return 180f / (1440 - dayLength); } }
            public bool IsHordeNight
            {
                get {
                    if (Day % HordeNightFrequency == 0)
                        return true;
                    else
                        return false;
                }
            }
            public int HordeNightFrequency = 7;

            [Range(4f, 0f)] // The higher this number, the slower the game sclock. So reverse the slider order.
            public float ClockSpeed = 1f;
            public TextMeshProUGUI Clock;

            public int Day = 1;
            [SerializeField] private int _timeOfDay; // Serialize so we can see in Inspector.

            public int TimeOfDay {

                get { return _timeOfDay; }
                set
                {

                    _timeOfDay = value;
                    // There are 1440 minutes in a day wrap our value back around to 0 when it goes over that.
                    if (_timeOfDay > 1439) {

                        _timeOfDay = 0;
                        Day++;

                    }

                    UpdateClock();

                    float rotAmount;

                    // The start of the "day" is zero rotation on the sunlight, so that's the most straightforward
                    // calculation.
                    if (_timeOfDay > Equinoxes[0].sunRise && _timeOfDay < Equinoxes[0].sunSet) {

                        rotAmount = (_timeOfDay - Equinoxes[0].sunRise) * sunDayRotationPerMinute;

                        // At the end of the "day" we switch to night rotation speed, but in order to keep the rotation
                        // seamless, we need to account for the daytime rotation as well.
                    }
                    else if (_timeOfDay >= Equinoxes[0].sunSet) {

                        // Calculate the amount of rotation through the day so far.
                        rotAmount = dayLength * sunDayRotationPerMinute;
                        // Add the rotation since the end of the day.
                        rotAmount += ((_timeOfDay - Equinoxes[0].sunRise - dayLength) * sunNightRotationPerMinute);

                        // Else we're at the start of a new day but because we're still in the same rotation cycle, we need to
                        // to account for all the previous rotation since dayStartTime the previous day.
                    }
                    else {

                        rotAmount = dayLength * sunDayRotationPerMinute; // Previous day's rotation.
                        rotAmount += (1440 - Equinoxes[0].sunSet) * sunNightRotationPerMinute; // Previous night's rotation.
                        rotAmount += _timeOfDay * sunNightRotationPerMinute; // Rotation since midnight.

                    }

                    World.Instance.sunObj.transform.eulerAngles = new Vector3(rotAmount, 0f, 0f);

                }
            }

            float secondCounter;



            void Update()
            {
            }

            public void UpdateTime() { //Updates the time
                // Increment TimeOfDay every second. Change 1f to speed up/slow down time. (2f would make days twice as long, 0.5f half as long).
                secondCounter += Time.deltaTime;
                if (secondCounter > ClockSpeed)
                {
                    TimeOfDay++;
                    secondCounter = 0;
                }
            }

            public void UpdateClock() {
                string dayText;
                dayText = Day.ToString();
                // Adding "D2" to the ToString() command ensures that there will always be two digits displayed.
                Clock.text = string.Format("DAY: {0} ", dayText);

            }
            //public void SetClockText(TextMeshProUGUI text) {
            //    Clock = text;
            //}
        }

        [System.Serializable]
        public class Equinox {
            public string Name;
            [Tooltip("Amount of minutes till sunrise")]
            public int sunRise;
            [Tooltip("Amount of minutes till sunset from 00:00")]
            public int sunSet;
        }
    }
}
