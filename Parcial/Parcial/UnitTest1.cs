using NUnit.Framework;
using System;

namespace Taller_POO.Tests
{
    [TestFixture]
    public class PruebasArbolDeComportamiento
    {
        [Test]

        //Test 1
        public void ArbolDeComportamiento_SoloDebeTenerUnRoot()
        {
            Root root = new Root();
            BehaviourTree arbol = new BehaviourTree(root);
            Assert.That(arbol, Is.Not.Null);
        }

        [Test]

        //Test 2
        public void Root_SoloPuedeTenerUnHijo_YNoOtroRoot()
        {
            Root root = new Root();
            Sequence secuencia = new Sequence();

            root.SetChild(secuencia);
            Assert.Throws<InvalidOperationException>(() => root.SetChild(new Selector()),
                "Root solo puede tener un hijo.");

            Assert.Throws<InvalidOperationException>(() => root.SetChild(new Root()),
                "Root no puede tener otro Root como hijo.");
        }

        [Test]

        //Test 3
        public void Composite_NoPuedeSerInstanciadoDirectamente()
        {
            Assert.That(typeof(Composite).IsAbstract, "Composite debe ser una clase abstracta.");
        }

        [Test]

        //Test 4
        public void Composite_NoPuedeTenerUnRootComoHijo()
        {
            Selector selector = new Selector();
            Root root = new Root();

            Assert.Throws<InvalidOperationException>(() => selector.AddChild(root),
                "Composite no puede tener Root como hijo.");
        }

        [Test]

        //Test 5
        public void TaskNode_NoPuedeSerInstanciadoDirectamente()
        {
            Assert.That(typeof(TaskNode).IsAbstract, "TaskNode debe ser una clase abstracta.");
        }

        [Test]

        //Test 6
        public void TaskNode_NoPuedeTenerHijos()
        {
            CheckEvenNumberTask tarea = new CheckEvenNumberTask(4);
            Assert.Throws<InvalidOperationException>(() => tarea.AddChild(new CheckEvenNumberTask(2)),
                "Task no puede tener hijos.");
        }


        [Test]

        //Test 7 
        public void Nodo_HerenciaCorrecta()
        {
            Assert.That(new Root(), Is.InstanceOf<Node>());
            Assert.That(new Sequence(), Is.InstanceOf<Node>());
            Assert.That(new Selector(), Is.InstanceOf<Node>());
            Assert.That(new CheckEvenNumberTask(4), Is.InstanceOf<Node>());
            Assert.That(new CheckDistanceTask(5, 10), Is.InstanceOf<Node>());

            Assert.That(new Sequence(), Is.InstanceOf<Composite>());
            Assert.That(new Selector(), Is.InstanceOf<Composite>());

            Assert.That(new CheckEvenNumberTask(4), Is.InstanceOf<TaskNode>());
            Assert.That(new MoveToTargetTask(new CheckDistanceTask(3, 10), 2.0f), Is.InstanceOf<TaskNode>());
        }

        [Test]

        //Test 8
        public void ArbolDeComportamiento_DebeTenerAlMenosUnRoot()
        {
            Assert.Throws<ArgumentNullException>(() => new BehaviourTree(null));
        }

        [Test]

        // Test 9
        public void Nodo_Execute_RetornaValoresCorrectos()
        {
            Root root = new Root();
            BehaviourTree arbol = new BehaviourTree(root);

            // Caso 1: Root vacío -> False
            Assert.That(arbol.Execute(), Is.False);

            // Caso 2: Root con una tarea que retorna True
            CheckEvenNumberTask tarea = new CheckEvenNumberTask(4);
            root.SetChild(tarea);
            Assert.That(arbol.Execute(), Is.True);

            // Caso 3: Root con Sequence [True, False] -> False
            Sequence secuencia = new Sequence();
            secuencia.AddChild(new CheckEvenNumberTask(2)); // True
            secuencia.AddChild(new CheckEvenNumberTask(3)); // False
            root.SetChild(secuencia);
            Assert.That(arbol.Execute(), Is.False);

            // Caso 4: Root con Selector [False, True] -> True
            Selector selector = new Selector();
            selector.AddChild(new CheckEvenNumberTask(3)); // False
            selector.AddChild(new CheckEvenNumberTask(4)); // True
            root.SetChild(selector);
            Assert.That(arbol.Execute(), Is.True);
        }
    }
}
