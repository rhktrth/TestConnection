using System.Collections.Generic;

namespace TestConnection {
    class TesterServerList : List<TesterServer> {
        public void AllStart() {
            foreach (TesterServer ts in this) {
                ts.Start();
            }
        }

        public void AllStop() {
            foreach (TesterServer ts in this) {
                ts.Stop();
            }
        }

        public void AllClear() {
            foreach (TesterServer ts in this) {
                ts.ClearCount();
            }
        }
    }
}
