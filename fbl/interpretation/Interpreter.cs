﻿using FBL.Interpretation.Modules;
using FBL.Parsing.Nodes;
using System;
using System.Collections.Generic;

namespace FBL.Interpretation
{
    public partial class Interpreter
    {
        private Context globalContext = new Context(null);
        private List<IModule> loadedModules = new List<IModule>();


        public Interpreter()
        {
            globalContext.Values["set"] =
                    new FunctionNode((i, c) => Set(i, globalContext)) { Parameter = new VariableNode("name") };
            globalContext.Values["get"] =
                new FunctionNode((i, c) => Get(i, globalContext)) { Parameter = new VariableNode("name") };
        }


        public ExpressionNode Run(CoreNode program)
        {
            var result = new ExpressionNode();

            foreach (var exp in program.code)
                result = Evaluate((dynamic)exp, GetGlobalContext());

            return result;
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

        public ExpressionNode SetVariable(string name, ExpressionNode value, Context context)
        {
            if (context == null) return new ExpressionNode();

            var ctx = context;
            while (ctx != null)
            {
                if (ctx.Values.ContainsKey(name))
                    return ctx.Values[name] = value;

                ctx = ctx.Parent;
            }

            context.Values.Add(name, value);
            return value;
        }
        public ExpressionNode GetVariable(string name, Context context)
        {
            var ctx = context;
            while (ctx != null)
            {
                if (ctx.Values.ContainsKey(name))
                    return ctx.Values[name];

                ctx = ctx.Parent;
            }

            return new ExpressionNode();
        }


        public Context GetGlobalContext() => globalContext;

        private ExpressionNode Evaluate(FunctionCallNode node, Context context)
        {
            ExpressionNode left = Evaluate((dynamic)node.CalleeExpression, context);

            if (left is FunctionNode leftFunc)
            {
                ExpressionNode right = Evaluate((dynamic)node.Argument, context);

                if (leftFunc.IsNative)
                    return leftFunc.Function(right, context);

                var subContext = new Context(leftFunc.Context);

                subContext.Values["set"] =
                    new FunctionNode((i, c) => Set(i, subContext)) { Parameter = new VariableNode("name") };
                subContext.Values["get"] =
                    new FunctionNode((i, c) => Get(i, subContext)) { Parameter = new VariableNode("name") };
                subContext.Values[leftFunc.Parameter?.Name ?? "it"] = right;

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

        public ExpressionNode Get(ExpressionNode input, Context context)
            => GetVariable(LanguageModule.ToString(input, context).StringValue, context);

        private FunctionNode Set(ExpressionNode input, Context context)
            => new FunctionNode(
                (v, c) => SetVariable(LanguageModule.ToString(input, context).StringValue, v?.Clone(), context)
            )
            { Parameter = new VariableNode("right") };

        private ExpressionNode Evaluate(BlockNode node, Context context)
        {
            ExpressionNode result = new ExpressionNode() { Context = context };

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
                {
                    // value = value ?? new ExpressionNode();
                    // value.Context = ctx;
                    return value;
                }
                ctx = ctx.Parent;
            }

            var expression = new ExpressionNode();
            context.Values.Add(node.Name, expression);
            expression.Context = context;
            return expression;
        }

        private ExpressionNode Evaluate(ExpressionNode node, Context context)
        {
            var value = node?.Clone() ?? new ExpressionNode();
            value.Context = context;
            return value;
        }
    }
}
