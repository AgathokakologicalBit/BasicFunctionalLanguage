using FBL.Parsing.Nodes;
using System;
using System.Collections.Generic;

namespace FBL.Interpretation
{
    public partial class Interpreter
    {
        private Context globalContext = new Context(null);
        private List<IModule> loadedModules = new List<IModule>();


        public void AddModule(IModule module)
        {
            module.OnLoad(this);
            loadedModules.Add(module);
        }

        public void SetVariable(string name, ExpressionNode value, Context context)
        {
            context.values.Add(name, value);
        }

        public ExpressionNode ChangeValue(ExpressionNode from, ExpressionNode to, Context context)
        {
            if (context == null) return null;

            foreach (var v in context?.values)
            {
                if (v.Value.Value == from)
                {
                    context.values[v.Key] = to;
                    return to;
                }
            }

            if (context.parent != null)
                return ChangeValue(from, to, context.parent);

            return new ExpressionNode();
        }

        public ExpressionNode Run(ExpressionNode node, StringNode data)
        {
            try
            {
                return Evaluate(new FunctionCallNode
                {
                    Argument = data,
                    CalleeExpression = node,
                    Context = globalContext
                }, globalContext);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return new ExpressionNode();
            }
        }

        public Context GetGlobalContext() => globalContext;


        public ExpressionNode Run(CoreNode program)
        {
            var result = new ExpressionNode();

            try
            {
                foreach (var exp in program.code)
                    result = Evaluate((dynamic)exp, GetGlobalContext());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return new ExpressionNode();
            }

            return result;
        }

        private ExpressionNode Evaluate(FunctionCallNode node, Context context)
        {
            ExpressionNode left = Evaluate((dynamic)node.CalleeExpression, context);
            if (left is FunctionNode left_func)
            {
                ExpressionNode right = Evaluate((dynamic)node.Argument, context);
                left_func.Context.values[left_func.Parameter?.Name ?? "it"] = right;

                if (left_func.IsNative)
                    return left_func.Function(right);

                return Evaluate((dynamic)left_func.Code, left_func.Context);
            }

            Console.WriteLine("Current context:");
            Console.WriteLine(context);

            throw new InvalidOperationException(
                $"calling '{node.CalleeExpression?.GetType().Name}'\n" +
                $"  evaluated to: {left?.ToString() ?? "null"}\n" +
                $"is impossible");
        }

        public ExpressionNode Run(string name, ExpressionNode args)
        {
            if (!globalContext.values.TryGetValue(name, out ExpressionNode node))
                return new ExpressionNode();

            if (!(node is FunctionNode f))
                return new ExpressionNode();


            try
            {
                return Evaluate(new FunctionCallNode
                {
                    Argument = args,
                    CalleeExpression = f,
                    Context = globalContext
                }, globalContext);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return new ExpressionNode();
        }

        private ExpressionNode Evaluate(BlockNode node, Context context)
        {
            ExpressionNode result = new ExpressionNode();

            foreach (var exp in node.Code)
                result = Evaluate((dynamic)exp, context);

            return result;
        }

        private ExpressionNode Evaluate(VariableNode node, Context context)
        {
            var ctx = context;
            while (ctx != null)
            {
                if (context.values.TryGetValue(node.Name, out ExpressionNode value))
                    return value;
                ctx = ctx.parent;
            }
            
            var expression = new ExpressionNode();
            context.values.Add(node.Name, expression);
            expression.Context = context;
            return expression;
        }

        private ExpressionNode Evaluate(ExpressionNode node, Context context)
        {
            var value = node?.Value?.Clone() ?? new ExpressionNode();
            value.Context = context;
            return value;
        }
    }
}
