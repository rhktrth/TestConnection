using System;

namespace TestConnection.Tests {
    internal static class TestAssert {
        public static void True(bool condition, string message) {
            if (!condition) {
                throw new InvalidOperationException("Assertion failed: " + message);
            }
        }

        public static void False(bool condition, string message) {
            True(!condition, message);
        }

        public static void Equal<T>(T expected, T actual, string message) {
            if (!object.Equals(expected, actual)) {
                throw new InvalidOperationException(
                    "Assertion failed: " + message + ". expected=" + expected + ", actual=" + actual);
            }
        }

        public static void Throws<T>(Action action, string message) where T : Exception {
            try {
                action();
            } catch (T) {
                return;
            }
            throw new InvalidOperationException(
                "Assertion failed: " + message + ". expected exception=" + typeof(T).Name);
        }
    }
}
