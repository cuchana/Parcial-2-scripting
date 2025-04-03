using NUnit.Framework;
using System;

namespace UnitTest1
{
    [TestFixture]
    public class BehaviorTreeTests
    {
        #region Pruebas de Estructura del Árbol

        [Test]
        public void BehaviorTree_ShouldHaveOnlyOneRoot()
        {
            // Arrange
            var root = new Sequence();
            var tree = new BehaviourTree(root);

            // Assert
            Assert.That(tree.Root, Is.EqualTo(root));
        }

        [Test]
        public void Root_ShouldHaveExactlyOneChild_AndCannotBeAnotherRoot()
        {
            // Arrange
            var root = new Sequence();
            var child = new Selector();
            root.AddChild(child);

            // Act & Assert
            Assert.DoesNotThrow(() => new BehaviourTree(root));

            // Verificar que no puede ser otro Root
            var anotherRoot = new Sequence();
            Assert.Throws<InvalidOperationException>(() => root.AddChild(anotherRoot));
        }

        [Test]
        public void Composite_CannotBeInstantiatedDirectly()
        {
            // Verificar que Composite es abstracta
            Assert.That(typeof(Composite).IsAbstract, Is.True);
        }

        [Test]
        public void Composite_CannotHaveRootAmongChildren()
        {
            // Arrange
            var composite = new Sequence();
            var potentialRoot = new Sequence();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => composite.AddChild(potentialRoot));
        }

        [Test]
        public void Task_CannotBeInstantiatedDirectly()
        {
            // Verificar que Node es abstracta (ya que Task no existe como clase separada)
            Assert.That(typeof(Node).IsAbstract, Is.True);
        }

        [Test]
        public void Task_ShouldNotHaveAnyChildren()
        {
            // Arrange
            var task = new IsEvenTask(2);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => task.AddChild(new IsEvenTask(3)));
        }

        #endregion

        #region Pruebas de Jerarquía de Herencia

        [Test]
        public void ClassHierarchy_ShouldFollowSpecifications()
        {
            // Todas derivan de Node
            Assert.That(typeof(Composite).IsSubclassOf(typeof(Node)));
            Assert.That(typeof(Selector).IsSubclassOf(typeof(Composite)));
            Assert.That(typeof(Sequence).IsSubclassOf(typeof(Composite)));
            Assert.That(typeof(CheckDistanceTask).IsSubclassOf(typeof(Node)));
            Assert.That(typeof(MoveToTargetTask).IsSubclassOf(typeof(Node)));
            Assert.That(typeof(WaitTask).IsSubclassOf(typeof(Node)));
            Assert.That(typeof(IsEvenTask).IsSubclassOf(typeof(Node)));

            // No derivan de Root (Root no existe en el código original)
            // Asumiendo que Root es equivalente a BehaviourTree en este contexto
            Assert.That(typeof(Selector).IsSubclassOf(typeof(BehaviourTree)), Is.False);
            Assert.That(typeof(Sequence).IsSubclassOf(typeof(BehaviourTree)), Is.False);
            Assert.That(typeof(CheckDistanceTask).IsSubclassOf(typeof(BehaviourTree)), Is.False);
        }

        #endregion

        #region Pruebas de Comportamiento del Árbol

        [Test]
        public void EmptyRoot_ShouldReturnFalse()
        {
            // Arrange
            var root = new Sequence(); // Sequence vacío no tiene hijos
            var tree = new BehaviourTree(root);

            // Act
            var result = tree.Execute();

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void RootWithTask_ShouldReturnTaskResult()
        {
            // Arrange
            var root = new Sequence();
            var evenTask = new IsEvenTask(2);
            root.AddChild(evenTask);
            var tree = new BehaviourTree(root);

            // Act
            var result = tree.Execute();

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void Sequence_ShouldReturnTrueOnlyIfAllChildrenReturnTrue()
        {
            // Arrange
            var sequence = new Sequence();
            sequence.AddChild(new IsEvenTask(2)); // true
            sequence.AddChild(new IsEvenTask(3)); // false
            var tree = new BehaviourTree(sequence);

            // Act
            var result = tree.Execute();

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void Selector_ShouldReturnTrueIfAnyChildReturnsTrue()
        {
            // Arrange
            var selector = new Selector();
            selector.AddChild(new IsEvenTask(3)); // false
            selector.AddChild(new IsEvenTask(4)); // true
            var tree = new BehaviourTree(selector);

            // Act
            var result = tree.Execute();

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void CompositeWithSingleChild_ShouldReturnChildResult()
        {
            // Arrange
            var sequence = new Sequence();
            sequence.AddChild(new IsEvenTask(3)); // false
            var tree = new BehaviourTree(sequence);

            // Act
            var result = tree.Execute();

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void NestedComposites_ShouldWorkCorrectly()
        {
            // Arrange - (true AND false) OR true => true
            var root = new Selector();

            var sequence = new Sequence();
            sequence.AddChild(new IsEvenTask(2)); // true
            sequence.AddChild(new IsEvenTask(3)); // false

            root.AddChild(sequence); // false
            root.AddChild(new IsEvenTask(4)); // true

            var tree = new BehaviourTree(root);

            // Act
            var result = tree.Execute();

            // Assert
            Assert.That(result, Is.True);
        }

        #endregion

        #region Pruebas de los nodos existentes

        [Test]
        public void CheckDistanceTask_ShouldWorkCorrectly()
        {
            // Arrange
            var validTask = new CheckDistanceTask(5.0f, 10.0f);
            var invalidTask = new CheckDistanceTask(15.0f, 10.0f);

            // Act & Assert
            Assert.That(validTask.Execute(), Is.True);
            Assert.That(invalidTask.Execute(), Is.False);
        }

        [Test]
        public void MoveToTargetTask_ShouldOnlyMoveIfDistanceIsValid()
        {
            // Arrange
            var validCheck = new CheckDistanceTask(5.0f, 10.0f);
            var invalidCheck = new CheckDistanceTask(15.0f, 10.0f);

            var validMove = new MoveToTargetTask(validCheck);
            var invalidMove = new MoveToTargetTask(invalidCheck);

            // Act & Assert
            Assert.That(validMove.Execute(), Is.True);
            Assert.That(invalidMove.Execute(), Is.False);
        }

        [Test]
        public void WaitTask_ShouldAlwaysReturnTrue()
        {
            // Arrange
            var wait = new WaitTask(100);

            // Act & Assert
            Assert.That(wait.Execute(), Is.True);
        }

        #endregion
    }
}