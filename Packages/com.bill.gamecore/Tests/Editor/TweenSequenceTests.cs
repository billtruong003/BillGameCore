using NUnit.Framework;

namespace BillGameCore.Tests
{
    public class TweenSequenceTests
    {
        TweenService _svc;

        [SetUp] public void SetUp() { _svc = new TweenService(); _svc.Initialize(); }
        [TearDown] public void TearDown() => _svc.Cleanup();

        [Test]
        public void StandaloneTween_AdvancesOncePerTick()
        {
            float v = 0f;
            _svc.Play(0f, 1f, 1f, x => v = x);
            _svc.Tick(0.25f);
            Assert.AreEqual(0.25f, v, 1e-4f);
        }

        [Test]
        public void SequencedTween_IsNotTickedTwice()
        {
            float v = 0f;
            _svc.Sequence().Append(_svc.Play(0f, 1f, 1f, x => v = x));
            _svc.Tick(0.25f);
            Assert.AreEqual(0.25f, v, 1e-4f);
            Assert.AreEqual(0, _svc.ActiveCount, "a sequenced tween must leave the service's active list");
        }

        [Test]
        public void Join_RunsInParallelWithItsAppend()
        {
            float a = 0f, b = 0f;
            _svc.Sequence()
                .Append(_svc.Play(0f, 1f, 1f, x => a = x))
                .Join(_svc.Play(0f, 1f, 1f, x => b = x));
            _svc.Tick(0.5f);
            Assert.AreEqual(0.5f, a, 1e-4f);
            Assert.AreEqual(0.5f, b, 1e-4f);
        }

        [Test]
        public void Append_WaitsForThePreviousGroup()
        {
            float a = 0f, b = -1f;
            bool done = false;
            _svc.Sequence()
                .Append(_svc.Play(0f, 1f, 1f, x => a = x))
                .Append(_svc.Play(0f, 1f, 1f, x => b = x))
                .OnComplete(() => done = true);
            _svc.Tick(1f);
            Assert.AreEqual(1f, a, 1e-4f);
            Assert.AreEqual(-1f, b, "second step must not start in the frame the first one ends");
            _svc.Tick(0.5f);
            Assert.AreEqual(0.5f, b, 1e-4f);
            _svc.Tick(0.5f);
            _svc.Tick(0f);
            Assert.IsTrue(done);
        }

        [Test]
        public void Callback_FiresBetweenSteps()
        {
            int calls = 0;
            float b = -1f;
            _svc.Sequence()
                .Append(_svc.Play(0f, 1f, 0.5f, _ => { }))
                .AppendCallback(() => calls++)
                .Append(_svc.Play(0f, 1f, 1f, x => b = x));
            _svc.Tick(0.5f);
            Assert.AreEqual(1, calls);
            _svc.Tick(0.5f);
            Assert.AreEqual(0.5f, b, 1e-4f);
        }

        [Test]
        public void KillTarget_StopsSequencesThatHoldTheTarget()
        {
            var owner = new object();
            float v = 0f;
            _svc.Sequence().Append(_svc.Play(0f, 1f, 1f, x => v = x).SetTarget(owner));
            _svc.Tick(0.25f);
            _svc.KillTarget(owner);
            _svc.Tick(0.25f);
            Assert.AreEqual(0.25f, v, 1e-4f);
        }
    }
}
