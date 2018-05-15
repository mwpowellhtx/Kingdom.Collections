using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Kingdom.Collections
{
    using Xunit;
    using static String;

    /// <summary>
    /// <see cref="Enumeration{T}"/> coverage reporter.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <inheritdoc />
    public interface IEnumerationCoverageReporter<in T> : IDisposable
        where T : Enumeration<T>
    {
        /// <summary>
        /// Gets or sets whether the Reporter is considered Enabled for purposes
        /// of the current testing context.
        /// </summary>
        bool Enabled { get; set; }

        /// <summary>
        /// Reports the <paramref name="names"/> that were tested.
        /// </summary>
        /// <param name="names"></param>
        /// <see cref="Report(T[])"/>
        void Report(params string[] names);

        /// <summary>
        /// Reports the <paramref name="values"/> that were tested.
        /// </summary>
        /// <param name="values"></param>
        /// <see cref="Report(string[])"/>
        void Report(params T[] values);
    }

    /// <summary>
    /// Implements the <see cref="Enumeration{T}"/> coverage reporter.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <inheritdoc />
    public class EnumerationCoverageReporter<T> : IEnumerationCoverageReporter<T>
        where T : Enumeration<T>
    {
        /// <summary>
        /// Gets or sets whether the Reporter is considered Enabled for purposes
        /// of the current testing context. Default value is true.
        /// </summary>
        /// <inheritdoc />
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Tallies the Tested Names.
        /// </summary>
        protected IDictionary<string, int> TestedNames { get; }
            = new ConcurrentDictionary<string, int>();

        /// <summary>
        /// Reports the <paramref name="names"/> that were tested.
        /// </summary>
        /// <param name="names"></param>
        /// <see cref="Report(T[])"/>
        /// <inheritdoc />
        public void Report(params string[] names)
        {
            Assert.NotEmpty(names);

            foreach (var name in names)
            {
                if (TestedNames.ContainsKey(name))
                {
                    TestedNames[name]++;
                }
                else
                {
                    TestedNames.Add(name, 1);
                }
            }
        }

        /// <summary>
        /// Reports the <paramref name="values"/> that were tested.
        /// </summary>
        /// <param name="values"></param>
        /// <see cref="Report(string[])"/>
        /// <inheritdoc />
        public void Report(params T[] values)
            => Report(values.Select(x => x.Name).ToArray());

        /// <summary>
        /// Gets whether the object IsDisposed.
        /// </summary>
        protected bool IsDisposed { get; private set; }

        /// <summary>
        /// Disposes the object.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            // ReSharper disable once InvertIf
            if (disposing && !IsDisposed)
            {
                if (Enabled)
                {
                    Enumeration<T>.Values.VerifyValuesCoverage(TestedNames);
                }
            }
        }

        /// <summary>
        /// Disposes the object.
        /// </summary>
        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            IsDisposed = true;
        }
    }

    /// <summary>
    /// Reporter extension methods provided for shorthand.
    /// </summary>
    public static class ReporterExtensionMethods
    {
        internal static void VerifyValuesCoverage<T>(this IEnumerable<T> values, IDictionary<string, int> coverage)
            where T : Enumeration<T>
        {
            Assert.NotNull(values);

            // ReSharper disable once InconsistentNaming
            var __values = values.ToArray();

            /* This is a hard exception. If this occurs, we have other problems to contend with.
             Think it through, there need to be at least One item in the Values array for this
             to be useful. */
            Assert.True(__values.Length > 0);

            try
            {
                // Then, we expect each of the Values to be Evaluated.
                Assert.Equal(__values.Length, coverage.Count);

                // Each of the Values shall be Evaluated at least Once.
                Assert.All(coverage.Values, count => Assert.True(count > 0));
            }
            catch (Exception ex)
            {
                // TODO: TBD: Assert inconclusive how? i.e. NUnit provides Assert.Inconclusive(...).
                var incomplete = __values.Select(x => x.Name).Except(coverage.Keys)
                    .Aggregate(Empty, (g, x) => IsNullOrEmpty(g) ? $"'{x}'" : $"{g}, '{x}'");

                // TODO: TBD: for lack of a better way of signaling, just throw the IOEX here...
                throw new InvalidOperationException($"Incomplete test coverage: [ {incomplete} ]", ex);
            }
        }
    }
}
