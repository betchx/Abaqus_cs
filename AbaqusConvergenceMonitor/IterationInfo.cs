using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;



namespace AbaqusConvergenceMonitor
{
  static class MatchAdapter
  {
    public static int to_i(this Match m, int pos = 1)
    {
      int res = 0;
      return int.TryParse(m.Groups[pos].Captures[0].Value, out res) ? res : 0;
    }
    public static double to_f(this Match m, int pos = 1)
    {
      double res = 0;
      return double.TryParse(m.Groups[pos].Captures[0].Value, out res) ? res : 0.0;
    }
  }

  enum State
  {
    Step,
    Attempt,
    Iteration,
    AverageForce,
    ResidualForce,
    DispIncrement,
    DispCorrection,
    ForceConverged,
    AverageMoment,
    ResidualMoment,
    RotIncrement,
    RotCorrection,
    MomentConverged,
    IterationEnded,
    NextIncrement,
  }

  class MsgMatcher
  {
    public State Current { get; private set; }
    public State MatchedState { get; private set; }
    public State UnmatchedState { get; private set; }
    public Regex Pattern { get; private set; }
    public Action<Match> MatchAction {get; private set;}
    public Regex SkipPattern { get; private set; }
    public MsgMatcher(State current, State matched, Regex pattern,
      Action<Match> action = null, State? unmatced = null, Regex skip_pattern = null)
    {
      Current = current;
      Pattern = pattern;
      MatchedState = matched;
      UnmatchedState = unmatced ?? current;
      MatchAction = action ?? ( (m) => {});
      SkipPattern = skip_pattern;
    }

    public Func<string, State> Function()
    {
      return (string s) => {
        if (SkipPattern != null) {
          if (SkipPattern.Match(s).Success)
            return Current;
        }
        var ans = Pattern.Match(s);
        if(ans.Success){
          MatchAction(ans);
          return MatchedState;
        }
        return UnmatchedState;
      };
    }
  }

  class IterationInfo
  {
    Dictionary<State, Func<string,State>> _pMap;
    State _current_state;

    public Action<IterationInfo> IterationEnded { get; set; }
    public Action<IterationInfo> IncrementEnded { get; set; }
    public Action<IterationInfo> StepEnded { get; set; }
    public int Step { get; private set; }
    public int Increment { get; private set; }
    public int Attempt { get; private set; }
    public int Iteration { get; private set; }
    public double AverageForce { get; private set; }
    public double ResidualForce { get; private set; }
    public bool ForceConverged { get; private set; }
    public double AverageMonent { get; private set; }
    public double ResidualMoment { get; private set; }
    public bool MomentConverged { get; private set; }
    public double TimeIncrement { get; private set; }



    readonly static Regex RE_STEP;// = new Regex(@"^ +S T E P +(\d+) +");
    readonly static Regex RE_ATTEMPT;// = new Regex(@"^ +INCREMENT +(\d+) STARTS. ATTEMPT NUMBER +(\d+), TIME INCREMENT  (\W+) +");
    readonly static Regex RE_ITERATION;// = new Regex(@" CONVERGENCE CHECKS FOR SEVERE DISCONTINUITY ITERATION +(\W+) +");
    readonly static Regex RE_AVERAGE_FORCE;// = new Regex(@" AVERAGE FORCE +(\W+) +");
    readonly static Regex RE_RESIDUAL_FORCE;// = new Regex(@"LARGEST RESIDUAL FORCE +(\W+)  +AT");
    readonly static Regex RE_FORCE_CONVERGED;// = new Regex(@" THE FORCE     EQUILIBRIUM EQUATIONS HAVE CONVERGED");
    readonly static Regex RE_AVERAGE_MOMENT;// = new Regex(@"AVERAGE MOMENT +(\W+) +TIME");
    readonly static Regex RE_RESIDUAL_MOMENT;// = new Regex(@"LARGEST RESIDUAL MOMENT +(\W+) +AT");
    readonly static Regex RE_MOMENT_CONVERGED;// = new Regex(@"THE MOMENT    EQUILIBRIUM EQUATIONS HAVE CONVERGED");
    readonly static Regex RE_NEXT;
    readonly static Regex RE_INSTANCE;

    static IterationInfo()
    {
      RE_STEP = new Regex(@"^ +S T E P +(\d+) +");
      RE_ATTEMPT = new Regex(@" +INCREMENT +(\d+) STARTS. ATTEMPT NUMBER +(\d+), TIME INCREMENT +(\S+)");
      RE_ITERATION = new Regex(@" CONVERGENCE CHECKS FOR SEVERE DISCONTINUITY ITERATION +(\S+)");
      RE_AVERAGE_FORCE = new Regex(@" AVERAGE FORCE +(\S+) +");
      RE_RESIDUAL_FORCE = new Regex(@"LARGEST RESIDUAL FORCE +(\S+)  +AT");
      RE_FORCE_CONVERGED = new Regex(@"THE FORCE + EQUILIBRIUM EQUATIONS HAVE CONVERGED");
      RE_INSTANCE = new Regex(@"INSTANCE");
      RE_AVERAGE_MOMENT = new Regex(@"AVERAGE MOMENT +(\S+) +TIME");
      RE_RESIDUAL_MOMENT = new Regex(@"LARGEST RESIDUAL MOMENT +(\S+) +AT");
      RE_MOMENT_CONVERGED = new Regex(@"THE MOMENT + EQUILIBRIUM EQUATIONS HAVE CONVERGED");
      RE_NEXT = new Regex(@"FRACTION OF STEP COMPLETED +(\S+)");
    }

    public IterationInfo()
    {
      var msgMatchers = new MsgMatcher[]{
        new MsgMatcher(State.Step, State.Attempt, RE_STEP, (m) => Step = m.to_i()),
        new MsgMatcher(State.Attempt, State.Iteration, RE_ATTEMPT, (m) => {
          Increment = m.to_i(1);
          Attempt = m.to_i(2);
          TimeIncrement = m.to_f(3);
          MomentConverged = false;
          ForceConverged = false;
        }),
        new MsgMatcher(State.Iteration, State.AverageForce, RE_ITERATION, (m) => Iteration = m.to_i(1)),
        new MsgMatcher(State.AverageForce, State.ResidualForce, RE_AVERAGE_FORCE, (m) => AverageForce = m.to_f()),
        new MsgMatcher(State.ResidualForce, State.DispIncrement, RE_RESIDUAL_FORCE, (m) => ResidualForce = m.to_f()),
        new MsgMatcher(State.DispIncrement, State.DispIncrement, RE_INSTANCE, null, State.DispCorrection),
        new MsgMatcher(State.DispCorrection, State.DispCorrection, RE_INSTANCE, null, State.ForceConverged),
        new MsgMatcher(State.ForceConverged, State.AverageMoment, RE_FORCE_CONVERGED, (m) => ForceConverged = true, State.AverageMoment, RE_INSTANCE),
        new MsgMatcher(State.AverageMoment, State.ResidualMoment, RE_AVERAGE_MOMENT, (m) => AverageMonent = m.to_f()),
        new MsgMatcher(State.ResidualMoment, State.RotIncrement, RE_RESIDUAL_MOMENT, (m) => ResidualMoment = m.to_f()),
        new MsgMatcher(State.RotIncrement, State.RotIncrement, RE_INSTANCE, null, State.RotCorrection),
        new MsgMatcher(State.RotCorrection, State.RotCorrection, RE_INSTANCE, null, State.MomentConverged),
        new MsgMatcher(State.MomentConverged, State.IterationEnded, RE_MOMENT_CONVERGED, (m) => MomentConverged = true, State.IterationEnded, RE_INSTANCE),
      };

      _pMap = new Dictionary<State, Func<string,State> >();
      foreach (var item in msgMatchers) {
        _pMap.Add(item.Current, item.Function());
      }

      // End of Iteration
      _pMap.Add(State.IterationEnded, s =>{
        IterationEnded.Invoke(this);
        return (ForceConverged && MomentConverged) ? State.NextIncrement : State.Iteration;});

      _pMap.Add(State.NextIncrement,
        s =>
        {
          var ans = RE_NEXT.Match(s);
          if (ans.Success) {
            if (ans.Groups[1].Value == "1.00") {
              StepEnded.Invoke(this);
              return State.Step;
            }
            return State.Attempt;
          }
          return State.NextIncrement;
        });

      Reset();
    }

    public bool ParseLine(string line)
    {
      if (line == null)
        return false;
      _current_state = _pMap[_current_state](line);
      return true;
    }

    public override string ToString()
    {
      var sb = new StringBuilder();
      sb.AppendFormat("I: {0,4},{1,5},{2,2}, {3,10}, {4,2}", Step, Increment, Attempt, TimeIncrement, Iteration);
      sb.AppendFormat(" F: {0,10},{1,10},{3,5}", AverageForce, ResidualForce, ForceConverged);
      sb.AppendFormat(" M: {0,10},{1,10},{3,5}", AverageMonent, ResidualMoment, MomentConverged);
      return sb.ToString();
    }

    public void Reset()
    {
      _current_state = State.Step;
    }
  }
}
