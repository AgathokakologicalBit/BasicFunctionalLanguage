using FBL.Parsing.Nodes;
using System;
using System.Collections.Generic;

namespace FBL.Interpretation
{
    public partial class Interpreter
    {
        private Context globalContext = new Context();
        private List<IModule> loadedModules = new List<IModule>();


        public Interpreter(Context context)
        {
            globalContext = context;

            globalContext.Values["get"].Context = globalContext;
            globalContext.Values["set"].Context = globalContext;
        }


        public ExpressionNode Run(CoreNode program)
        {
            program.Code.Context = globalContext;
            return Evaluate((dynamic)program.Code, globalContext);
        }

        public ExpressionNode Run(ExpressionNode node, StringNode data)
        {
            return Evaluate(new FunctionCallNode
            {
                Argument = data,
                CalleeExpression = node,
                Context = globalContext
            }, globalContext);
        }

        public ExpressionNode Run(string name, ExpressionNode args)
        {
            if (!globalContext.Values.TryGetValue(name, out ExpressionNode node))
                return new ExpressionNode();

            if (!(node is FunctionNode f))
                return new ExpressionNode();

            return Evaluate(new FunctionCallNode
            {
                Argument = args,
                CalleeExpression = f,
                Context = globalContext
            }, globalContext);
        }


        public void AddModule(IModule module)
        {
            module.OnLoad(this);
            loadedModules.Add(module);
        }


        public Context GetGlobalContext() => globalContext;

        private ExpressionNode Evaluate(FunctionCallNode node, Context context)
        {
            ExpressionNode left = Evaluate((dynamic)node.CalleeExpression, context);

            if (left is FunctionNode leftFunc)
            {
                ExpressionNode right = Evaluate((dynamic)node.Argument, context);

                if (leftFunc.Function != null)
                    return leftFunc.Function(right, leftFunc.Context);

                var subContext = leftFunc.Context.Clone();
                // subContext.Parent = context;
                subContext.Values[leftFunc.Parameter.Name] = right;

                return Evaluate((dynamic)leftFunc.Code, subContext);
            }

            string name = "";
            if (node.CalleeExpression is VariableNode callee_var)
                name = $"with name '{callee_var.Name}'";

            throw new InvalidOperationException(
                $"calling '{node.CalleeExpression?.GetType().Name ?? "null"}' {name}\n" +
                $"  evaluated to {left?.GetType().Name}: {left?.ToString() ?? "null"}\n" +
                $"is impossible");
        }


        private ExpressionNode Evaluate(BlockNode node, Context context)
        {
            ExpressionNode result = null;

            foreach (var exp in node.Code)
                result = Evaluate((dynamic)exp, context);

            return result;
        }

        private ExpressionNode Evaluate(VariableNode node, Context context)
        {
            var ctx = context;
            while (ctx != null)
            {
                if (ctx.Values.TryGetValue(node.Name, out ExpressionNode value))
                    return value;

                ctx = ctx.Parent;
            }

            return new ExpressionNode();
        }

        private ExpressionNode Evaluate(ExpressionNode node, Context context)
        {
            var value = node?.Clone() ?? new ExpressionNode();
            if (value is FunctionNode)
            {
                value.Context = value.Context.Clone();
                value.Context.Parent = context;
            }

            return value;
        }
    }
}
